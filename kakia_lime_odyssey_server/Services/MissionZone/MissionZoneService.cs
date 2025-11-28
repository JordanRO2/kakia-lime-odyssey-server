/// <summary>
/// Service for managing instanced mission zones (dungeons, raids, etc).
/// </summary>
/// <remarks>
/// Handles mission zone creation queue, instance lifecycle, player transfers,
/// and party-based mission zone coordination.
/// Uses: SC_CREATING_MISSION_ZONE for queue updates, SC_CANCELED_CREATE_MISSION_ZONE for cancel confirmation.
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Entities.Monsters;
using kakia_lime_odyssey_server.Network;
using System.Collections.Concurrent;

namespace kakia_lime_odyssey_server.Services.MissionZone;

/// <summary>
/// Definition of a mission zone template.
/// </summary>
public class MissionZoneDefinition
{
	/// <summary>Unique ID for this mission zone type.</summary>
	public int MissionTypeId { get; set; }

	/// <summary>Display name of the mission.</summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>Base zone type ID to use as template.</summary>
	public uint TemplateZoneTypeId { get; set; }

	/// <summary>Minimum level required to enter.</summary>
	public int MinLevel { get; set; } = 1;

	/// <summary>Maximum level allowed (0 = no limit).</summary>
	public int MaxLevel { get; set; }

	/// <summary>Minimum party size required (1 = solo allowed).</summary>
	public int MinPartySize { get; set; } = 1;

	/// <summary>Maximum party size allowed.</summary>
	public int MaxPartySize { get; set; } = 5;

	/// <summary>Time limit in minutes (0 = no limit).</summary>
	public int TimeLimitMinutes { get; set; }

	/// <summary>Entry cost in Peder.</summary>
	public int EntryCost { get; set; }

	/// <summary>Spawn position for players entering.</summary>
	public FPOS SpawnPosition { get; set; }

	/// <summary>Monster spawn configuration for this mission.</summary>
	public List<MissionSpawnConfig> Spawns { get; set; } = new();

	/// <summary>Reward configuration.</summary>
	public MissionRewardConfig? Rewards { get; set; }
}

/// <summary>
/// Monster spawn configuration for a mission zone.
/// </summary>
public class MissionSpawnConfig
{
	/// <summary>Monster type ID to spawn.</summary>
	public int MonsterTypeId { get; set; }

	/// <summary>Number of monsters to spawn.</summary>
	public int Count { get; set; } = 1;

	/// <summary>Spawn position.</summary>
	public FPOS Position { get; set; }

	/// <summary>Spawn radius for random positioning.</summary>
	public float SpawnRadius { get; set; } = 5.0f;

	/// <summary>Whether these monsters are a boss wave.</summary>
	public bool IsBoss { get; set; }

	/// <summary>Wave number (for sequential spawning).</summary>
	public int WaveNumber { get; set; }
}

/// <summary>
/// Reward configuration for completing a mission zone.
/// </summary>
public class MissionRewardConfig
{
	/// <summary>Base experience reward.</summary>
	public int BaseExp { get; set; }

	/// <summary>Base Peder reward.</summary>
	public int BasePeder { get; set; }

	/// <summary>Guaranteed item rewards (item type IDs).</summary>
	public List<int> GuaranteedItems { get; set; } = new();

	/// <summary>Possible bonus item rewards with drop rates.</summary>
	public Dictionary<int, float> BonusItems { get; set; } = new();
}

/// <summary>
/// Active mission zone instance.
/// </summary>
public class MissionZoneInstance
{
	/// <summary>Unique instance ID.</summary>
	public Guid InstanceId { get; set; } = Guid.NewGuid();

	/// <summary>Generated area instance ID for game use.</summary>
	public uint AreaInstId { get; set; }

	/// <summary>Mission definition reference.</summary>
	public MissionZoneDefinition Definition { get; set; } = null!;

	/// <summary>When this instance was created.</summary>
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	/// <summary>When this instance expires.</summary>
	public DateTime? ExpiresAt { get; set; }

	/// <summary>Player IDs currently in this instance.</summary>
	public List<long> PlayerIds { get; set; } = new();

	/// <summary>Party ID if this is a party mission.</summary>
	public int? PartyId { get; set; }

	/// <summary>Spawned monsters in this instance.</summary>
	public List<Monster> Monsters { get; set; } = new();

	/// <summary>Current wave number (for wave-based missions).</summary>
	public int CurrentWave { get; set; }

	/// <summary>Whether the mission has been completed.</summary>
	public bool IsCompleted { get; set; }

	/// <summary>Whether the mission has failed.</summary>
	public bool IsFailed { get; set; }

	/// <summary>State of the mission.</summary>
	public MissionState State { get; set; } = MissionState.Active;
}

/// <summary>
/// State of a mission zone instance.
/// </summary>
public enum MissionState
{
	/// <summary>Mission is in progress.</summary>
	Active,
	/// <summary>Mission completed successfully.</summary>
	Completed,
	/// <summary>Mission failed (time out, party wipe, etc).</summary>
	Failed,
	/// <summary>Mission is being cleaned up.</summary>
	Closing
}

/// <summary>
/// Player's request to create/join a mission zone.
/// </summary>
public class MissionZoneRequest
{
	/// <summary>Player ID requesting the mission.</summary>
	public long PlayerId { get; set; }

	/// <summary>Player's name.</summary>
	public string PlayerName { get; set; } = string.Empty;

	/// <summary>Mission type ID requested.</summary>
	public int MissionTypeId { get; set; }

	/// <summary>When the request was made.</summary>
	public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

	/// <summary>Position in queue.</summary>
	public int QueuePosition { get; set; }

	/// <summary>Party ID if this is a party request.</summary>
	public int? PartyId { get; set; }
}

/// <summary>
/// Service for managing mission zones (instanced dungeons).
/// </summary>
public class MissionZoneService
{
	/// <summary>Maximum concurrent mission instances.</summary>
	private const int MaxConcurrentInstances = 100;

	/// <summary>Queue update interval in milliseconds.</summary>
	private const int QueueUpdateIntervalMs = 5000;

	/// <summary>Mission zone definitions by type ID.</summary>
	private readonly Dictionary<int, MissionZoneDefinition> _definitions = new();

	/// <summary>Active mission instances by instance ID.</summary>
	private readonly ConcurrentDictionary<Guid, MissionZoneInstance> _instances = new();

	/// <summary>Queue of pending mission requests by mission type.</summary>
	private readonly ConcurrentDictionary<int, ConcurrentQueue<MissionZoneRequest>> _queues = new();

	/// <summary>Player ID to current request mapping.</summary>
	private readonly ConcurrentDictionary<long, MissionZoneRequest> _playerRequests = new();

	/// <summary>Player ID to current instance mapping.</summary>
	private readonly ConcurrentDictionary<long, Guid> _playerInstances = new();

	/// <summary>Counter for generating area instance IDs.</summary>
	private uint _nextAreaInstId = 1000000;

	/// <summary>
	/// Initializes the mission zone service with default definitions.
	/// </summary>
	public MissionZoneService()
	{
		InitializeDefaultMissions();
		Logger.Log("[MISSION] MissionZoneService initialized", LogLevel.Information);
	}

	/// <summary>
	/// Initializes default mission zone definitions.
	/// </summary>
	private void InitializeDefaultMissions()
	{
		// Example mission zone - would normally come from XML data
		_definitions[1001] = new MissionZoneDefinition
		{
			MissionTypeId = 1001,
			Name = "Training Dungeon",
			TemplateZoneTypeId = 100001, // Instance zone template
			MinLevel = 10,
			MaxLevel = 20,
			MinPartySize = 1,
			MaxPartySize = 3,
			TimeLimitMinutes = 30,
			EntryCost = 100,
			SpawnPosition = new FPOS { x = 100, y = 0, z = 100 },
			Spawns = new List<MissionSpawnConfig>
			{
				new() { MonsterTypeId = 1001, Count = 3, WaveNumber = 1 },
				new() { MonsterTypeId = 1002, Count = 2, WaveNumber = 2 },
				new() { MonsterTypeId = 1003, Count = 1, IsBoss = true, WaveNumber = 3 }
			},
			Rewards = new MissionRewardConfig
			{
				BaseExp = 5000,
				BasePeder = 1000,
				GuaranteedItems = new List<int> { 10001 }
			}
		};

		_definitions[1002] = new MissionZoneDefinition
		{
			MissionTypeId = 1002,
			Name = "Goblin Cave",
			TemplateZoneTypeId = 100002,
			MinLevel = 20,
			MaxLevel = 35,
			MinPartySize = 3,
			MaxPartySize = 5,
			TimeLimitMinutes = 45,
			EntryCost = 500,
			SpawnPosition = new FPOS { x = 50, y = 0, z = 50 },
			Rewards = new MissionRewardConfig
			{
				BaseExp = 15000,
				BasePeder = 5000
			}
		};

		Logger.Log($"[MISSION] Initialized {_definitions.Count} mission definitions", LogLevel.Information);
	}

	/// <summary>
	/// Registers a mission zone definition.
	/// </summary>
	public void RegisterMission(MissionZoneDefinition definition)
	{
		_definitions[definition.MissionTypeId] = definition;
		Logger.Log($"[MISSION] Registered mission: {definition.Name} (ID: {definition.MissionTypeId})", LogLevel.Debug);
	}

	/// <summary>
	/// Gets a mission zone definition.
	/// </summary>
	public MissionZoneDefinition? GetMissionDefinition(int missionTypeId)
	{
		return _definitions.TryGetValue(missionTypeId, out var def) ? def : null;
	}

	/// <summary>
	/// Checks if a player can enter a mission zone.
	/// </summary>
	public (bool CanEnter, string Reason) CanEnterMission(PlayerClient pc, int missionTypeId)
	{
		var def = GetMissionDefinition(missionTypeId);
		if (def == null)
		{
			return (false, "Mission zone not found.");
		}

		var character = pc.GetCurrentCharacter();
		if (character == null)
		{
			return (false, "Character not found.");
		}

		int playerLevel = character.status.combatJob.lv;

		// Check level requirements
		if (playerLevel < def.MinLevel)
		{
			return (false, $"Level too low. Required: {def.MinLevel}");
		}

		if (def.MaxLevel > 0 && playerLevel > def.MaxLevel)
		{
			return (false, $"Level too high. Maximum: {def.MaxLevel}");
		}

		// Check if already in a mission
		if (_playerInstances.ContainsKey(pc.GetId()))
		{
			return (false, "Already in a mission zone.");
		}

		// Check if already in queue
		if (_playerRequests.ContainsKey(pc.GetId()))
		{
			return (false, "Already in mission queue.");
		}

		// Check entry cost
		var inventory = pc.GetInventory();
		if (inventory.WalletPeder < def.EntryCost)
		{
			return (false, $"Not enough Peder. Required: {def.EntryCost}");
		}

		// Check party size if required
		if (def.MinPartySize > 1)
		{
			var party = LimeServer.PartyService.GetParty(pc);
			if (party == null)
			{
				return (false, $"Party required. Minimum size: {def.MinPartySize}");
			}

			int partySize = party.Members.Count;
			if (partySize < def.MinPartySize)
			{
				return (false, $"Party too small. Required: {def.MinPartySize}");
			}

			if (partySize > def.MaxPartySize)
			{
				return (false, $"Party too large. Maximum: {def.MaxPartySize}");
			}
		}

		return (true, string.Empty);
	}

	/// <summary>
	/// Requests to create/join a mission zone.
	/// </summary>
	public bool RequestMissionZone(PlayerClient pc, int missionTypeId)
	{
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		long playerId = pc.GetId();

		var (canEnter, reason) = CanEnterMission(pc, missionTypeId);
		if (!canEnter)
		{
			Logger.Log($"[MISSION] {playerName} denied mission {missionTypeId}: {reason}", LogLevel.Warning);
			return false;
		}

		// Get or create queue for this mission type
		var queue = _queues.GetOrAdd(missionTypeId, _ => new ConcurrentQueue<MissionZoneRequest>());

		// Create request
		var request = new MissionZoneRequest
		{
			PlayerId = playerId,
			PlayerName = playerName,
			MissionTypeId = missionTypeId,
			QueuePosition = queue.Count + 1
		};

		// Check if party request
		var party = LimeServer.PartyService.GetParty(pc);
		if (party != null)
		{
			request.PartyId = (int)party.Id;
		}

		// Add to queue
		queue.Enqueue(request);
		_playerRequests[playerId] = request;

		// Send queue status
		SendQueueStatus(pc, request.QueuePosition);

		Logger.Log($"[MISSION] {playerName} queued for mission {missionTypeId} (position {request.QueuePosition})", LogLevel.Information);

		// Try to process queue immediately
		ProcessQueue(missionTypeId);

		return true;
	}

	/// <summary>
	/// Cancels a pending mission zone request.
	/// </summary>
	public bool CancelMissionRequest(PlayerClient pc)
	{
		long playerId = pc.GetId();
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		if (_playerRequests.TryRemove(playerId, out var request))
		{
			// Send cancel confirmation
			SendCancelConfirmation(pc);

			Logger.Log($"[MISSION] {playerName} canceled mission request for {request.MissionTypeId}", LogLevel.Information);
			return true;
		}

		Logger.Log($"[MISSION] {playerName} has no pending mission request to cancel", LogLevel.Warning);
		return false;
	}

	/// <summary>
	/// Processes the queue for a mission type and creates instances as needed.
	/// </summary>
	private void ProcessQueue(int missionTypeId)
	{
		if (!_queues.TryGetValue(missionTypeId, out var queue))
		{
			return;
		}

		var def = GetMissionDefinition(missionTypeId);
		if (def == null)
		{
			return;
		}

		// Check if we can create more instances
		if (_instances.Count >= MaxConcurrentInstances)
		{
			Logger.Log("[MISSION] Maximum concurrent instances reached", LogLevel.Warning);
			return;
		}

		// Try to dequeue and create instance
		while (queue.TryPeek(out var request))
		{
			// Check if player is still waiting
			if (!_playerRequests.ContainsKey(request.PlayerId))
			{
				queue.TryDequeue(out _);
				continue;
			}

			// Check if player is still online
			var pc = LimeServer.PlayerClients.FirstOrDefault(p => p.GetId() == request.PlayerId);
			if (pc == null)
			{
				queue.TryDequeue(out _);
				_playerRequests.TryRemove(request.PlayerId, out _);
				continue;
			}

			// Create instance
			var instance = CreateMissionInstance(def, request);
			if (instance != null)
			{
				queue.TryDequeue(out _);
				_playerRequests.TryRemove(request.PlayerId, out _);

				// Transfer player to instance
				TransferPlayerToInstance(pc, instance);
			}

			break;
		}
	}

	/// <summary>
	/// Creates a new mission zone instance.
	/// </summary>
	private MissionZoneInstance? CreateMissionInstance(MissionZoneDefinition def, MissionZoneRequest request)
	{
		var instance = new MissionZoneInstance
		{
			AreaInstId = _nextAreaInstId++,
			Definition = def
		};

		// Set expiration if time limited
		if (def.TimeLimitMinutes > 0)
		{
			instance.ExpiresAt = DateTime.UtcNow.AddMinutes(def.TimeLimitMinutes);
		}

		// Add party members if this is a party mission
		if (request.PartyId.HasValue)
		{
			instance.PartyId = request.PartyId;
			// Party member handling would be done here
		}

		_instances[instance.InstanceId] = instance;

		Logger.Log($"[MISSION] Created instance {instance.InstanceId:N} for mission {def.Name}", LogLevel.Information);

		return instance;
	}

	/// <summary>
	/// Transfers a player into a mission zone instance.
	/// </summary>
	private void TransferPlayerToInstance(PlayerClient pc, MissionZoneInstance instance)
	{
		long playerId = pc.GetId();
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		// Deduct entry cost
		var def = instance.Definition;
		if (def.EntryCost > 0)
		{
			LimeServer.CurrencyService.RemovePeder(pc, def.EntryCost);
		}

		// Track player in instance
		instance.PlayerIds.Add(playerId);
		_playerInstances[playerId] = instance.InstanceId;

		// Use zone transfer service to move player
		LimeServer.ZoneTransferService.TransferToZone(
			pc,
			def.TemplateZoneTypeId,
			def.SpawnPosition
		);

		Logger.Log($"[MISSION] {playerName} entered mission {def.Name} (Instance: {instance.InstanceId:N})", LogLevel.Information);
	}

	/// <summary>
	/// Removes a player from their current mission zone.
	/// </summary>
	public void RemovePlayerFromMission(long playerId)
	{
		if (!_playerInstances.TryRemove(playerId, out var instanceId))
		{
			return;
		}

		if (_instances.TryGetValue(instanceId, out var instance))
		{
			instance.PlayerIds.Remove(playerId);

			// Check if instance is now empty
			if (instance.PlayerIds.Count == 0 && !instance.IsCompleted)
			{
				CleanupInstance(instanceId);
			}
		}
	}

	/// <summary>
	/// Marks a mission as completed and handles rewards.
	/// </summary>
	public void CompleteMission(Guid instanceId)
	{
		if (!_instances.TryGetValue(instanceId, out var instance))
		{
			return;
		}

		instance.IsCompleted = true;
		instance.State = MissionState.Completed;

		var def = instance.Definition;

		// Grant rewards to all players in instance
		foreach (var playerId in instance.PlayerIds.ToList())
		{
			var pc = LimeServer.PlayerClients.FirstOrDefault(p => p.GetId() == playerId);
			if (pc != null)
			{
				GrantMissionRewards(pc, def);
			}
		}

		Logger.Log($"[MISSION] Mission {def.Name} completed (Instance: {instanceId:N})", LogLevel.Information);

		// Schedule cleanup
		_ = Task.Run(async () =>
		{
			await Task.Delay(30000); // 30 seconds to collect loot
			CleanupInstance(instanceId);
		});
	}

	/// <summary>
	/// Grants mission completion rewards to a player.
	/// </summary>
	private void GrantMissionRewards(PlayerClient pc, MissionZoneDefinition def)
	{
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		if (def.Rewards == null)
		{
			return;
		}

		// Grant experience
		if (def.Rewards.BaseExp > 0)
		{
			var character = pc.GetCurrentCharacter();
			if (character != null)
			{
				character.status.combatJob.exp += (uint)def.Rewards.BaseExp;
				// Send exp update packet would go here
			}
		}

		// Grant Peder
		if (def.Rewards.BasePeder > 0)
		{
			LimeServer.CurrencyService.AddPeder(pc, def.Rewards.BasePeder);
		}

		// Grant items
		foreach (var itemTypeId in def.Rewards.GuaranteedItems)
		{
			// Would create and add item to inventory here
			Logger.Log($"[MISSION] {playerName} received reward item {itemTypeId}", LogLevel.Debug);
		}

		Logger.Log($"[MISSION] {playerName} received mission rewards", LogLevel.Information);
	}

	/// <summary>
	/// Marks a mission as failed.
	/// </summary>
	public void FailMission(Guid instanceId, string reason)
	{
		if (!_instances.TryGetValue(instanceId, out var instance))
		{
			return;
		}

		instance.IsFailed = true;
		instance.State = MissionState.Failed;

		Logger.Log($"[MISSION] Mission failed (Instance: {instanceId:N}): {reason}", LogLevel.Information);

		// Teleport all players out
		foreach (var playerId in instance.PlayerIds.ToList())
		{
			var pc = LimeServer.PlayerClients.FirstOrDefault(p => p.GetId() == playerId);
			if (pc != null)
			{
				// Return to town
				LimeServer.ZoneTransferService.WarpInZone(pc, new FPOS { x = 0, y = 0, z = 0 });
			}
		}

		CleanupInstance(instanceId);
	}

	/// <summary>
	/// Cleans up a mission zone instance.
	/// </summary>
	private void CleanupInstance(Guid instanceId)
	{
		if (!_instances.TryRemove(instanceId, out var instance))
		{
			return;
		}

		instance.State = MissionState.Closing;

		// Remove all player associations
		foreach (var playerId in instance.PlayerIds)
		{
			_playerInstances.TryRemove(playerId, out _);
		}

		// Cleanup monsters
		instance.Monsters.Clear();

		Logger.Log($"[MISSION] Instance {instanceId:N} cleaned up", LogLevel.Information);
	}

	/// <summary>
	/// Gets the instance a player is currently in.
	/// </summary>
	public MissionZoneInstance? GetPlayerInstance(long playerId)
	{
		if (_playerInstances.TryGetValue(playerId, out var instanceId))
		{
			return _instances.TryGetValue(instanceId, out var instance) ? instance : null;
		}
		return null;
	}

	/// <summary>
	/// Checks if a player is in a mission zone.
	/// </summary>
	public bool IsInMissionZone(long playerId)
	{
		return _playerInstances.ContainsKey(playerId);
	}

	/// <summary>
	/// Updates expired mission instances (called from server tick).
	/// </summary>
	public void UpdateExpiredInstances()
	{
		var now = DateTime.UtcNow;

		foreach (var kvp in _instances)
		{
			var instance = kvp.Value;

			if (instance.ExpiresAt.HasValue && now >= instance.ExpiresAt.Value)
			{
				FailMission(kvp.Key, "Time limit exceeded");
			}
		}
	}

	/// <summary>
	/// Sends SC_CREATING_MISSION_ZONE queue status to player.
	/// </summary>
	private void SendQueueStatus(PlayerClient pc, int queuePosition)
	{
		int activeCount = _instances.Count;
		int waitingCount = _queues.Values.Sum(q => q.Count);

		var packet = new SC_CREATING_MISSION_ZONE
		{
			order = queuePosition,
			waiting = waitingCount,
			created = activeCount,
			createLimit = MaxConcurrentInstances
		};

		using PacketWriter pw = new();
		pw.Write(packet);
		pc.Send(pw.ToPacket(), default).Wait();
	}

	/// <summary>
	/// Sends SC_CANCELED_CREATE_MISSION_ZONE to player.
	/// </summary>
	private void SendCancelConfirmation(PlayerClient pc)
	{
		var packet = new SC_CANCELED_CREATE_MISSION_ZONE();

		using PacketWriter pw = new();
		pw.Write(packet);
		pc.Send(pw.ToPacket(), default).Wait();
	}

	/// <summary>
	/// Gets all active mission instances.
	/// </summary>
	public IEnumerable<MissionZoneInstance> GetActiveInstances()
	{
		return _instances.Values.Where(i => i.State == MissionState.Active);
	}

	/// <summary>
	/// Gets mission zone statistics.
	/// </summary>
	public (int ActiveInstances, int TotalPlayers, int QueuedPlayers) GetStatistics()
	{
		int active = _instances.Count;
		int players = _playerInstances.Count;
		int queued = _playerRequests.Count;

		return (active, players, queued);
	}
}
