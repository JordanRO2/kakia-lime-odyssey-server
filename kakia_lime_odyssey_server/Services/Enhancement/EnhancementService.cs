/// <summary>
/// Service for managing item enhancement/upgrade operations.
/// </summary>
/// <remarks>
/// Handles item grade upgrades, enhancement materials, success/failure calculations,
/// protection items, and grade penalties on failure.
/// Extends the basic composition system with full enhancement features.
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_server.Models;
using kakia_lime_odyssey_server.Network;
using System.Collections.Concurrent;
using ItemModel = kakia_lime_odyssey_server.Models.Item;

namespace kakia_lime_odyssey_server.Services.Enhancement;

/// <summary>
/// Types of enhancement operations.
/// </summary>
public enum EnhancementType
{
	/// <summary>Standard grade upgrade (+1).</summary>
	GradeUpgrade,
	/// <summary>Safe upgrade (no destruction on failure).</summary>
	SafeUpgrade,
	/// <summary>Protected upgrade (no grade loss on failure).</summary>
	ProtectedUpgrade,
	/// <summary>Blessed upgrade (higher success rate).</summary>
	BlessedUpgrade
}

/// <summary>
/// Result of an enhancement attempt.
/// </summary>
public enum EnhancementOutcome
{
	/// <summary>Enhancement succeeded, grade increased.</summary>
	Success,
	/// <summary>Enhancement failed, no change.</summary>
	Failed,
	/// <summary>Enhancement failed and grade decreased.</summary>
	Degraded,
	/// <summary>Enhancement failed and item was destroyed.</summary>
	Destroyed
}

/// <summary>
/// Result of an enhancement operation.
/// </summary>
public class EnhancementResult
{
	/// <summary>The outcome of the enhancement.</summary>
	public EnhancementOutcome Outcome { get; set; }

	/// <summary>Whether the enhancement succeeded.</summary>
	public bool Success => Outcome == EnhancementOutcome.Success;

	/// <summary>New grade after enhancement (if applicable).</summary>
	public int NewGrade { get; set; }

	/// <summary>Previous grade before enhancement.</summary>
	public int PreviousGrade { get; set; }

	/// <summary>Error message if validation failed.</summary>
	public string ErrorMessage { get; set; } = string.Empty;

	/// <summary>The success rate that was calculated.</summary>
	public int SuccessRate { get; set; }

	/// <summary>The roll that was made.</summary>
	public int Roll { get; set; }

	/// <summary>Creates a success result.</summary>
	public static EnhancementResult Succeeded(int previousGrade, int newGrade, int rate, int roll) => new()
	{
		Outcome = EnhancementOutcome.Success,
		PreviousGrade = previousGrade,
		NewGrade = newGrade,
		SuccessRate = rate,
		Roll = roll
	};

	/// <summary>Creates a failure result.</summary>
	public static EnhancementResult FailedNoChange(int grade, int rate, int roll) => new()
	{
		Outcome = EnhancementOutcome.Failed,
		PreviousGrade = grade,
		NewGrade = grade,
		SuccessRate = rate,
		Roll = roll
	};

	/// <summary>Creates a degraded result.</summary>
	public static EnhancementResult FailedDegraded(int previousGrade, int newGrade, int rate, int roll) => new()
	{
		Outcome = EnhancementOutcome.Degraded,
		PreviousGrade = previousGrade,
		NewGrade = newGrade,
		SuccessRate = rate,
		Roll = roll
	};

	/// <summary>Creates a destroyed result.</summary>
	public static EnhancementResult FailedDestroyed(int grade, int rate, int roll) => new()
	{
		Outcome = EnhancementOutcome.Destroyed,
		PreviousGrade = grade,
		NewGrade = 0,
		SuccessRate = rate,
		Roll = roll
	};

	/// <summary>Creates an error result.</summary>
	public static EnhancementResult Error(string message) => new()
	{
		Outcome = EnhancementOutcome.Failed,
		ErrorMessage = message
	};
}

/// <summary>
/// Configuration for enhancement at a specific grade level.
/// </summary>
public class EnhancementLevelConfig
{
	/// <summary>The grade level this config applies to.</summary>
	public int Grade { get; set; }

	/// <summary>Base success rate percentage.</summary>
	public int BaseSuccessRate { get; set; }

	/// <summary>Chance to destroy item on failure.</summary>
	public int DestructionChance { get; set; }

	/// <summary>Chance to degrade item on failure (if not destroyed).</summary>
	public int DegradationChance { get; set; }

	/// <summary>How many grades to lose on degradation.</summary>
	public int DegradationAmount { get; set; } = 1;

	/// <summary>Whether this is a safe level (no destruction or degradation).</summary>
	public bool IsSafeLevel { get; set; }

	/// <summary>Enhancement cost in Peder.</summary>
	public int Cost { get; set; }
}

/// <summary>
/// Active enhancement session.
/// </summary>
public class EnhancementSession
{
	/// <summary>Inventory slot of item being enhanced.</summary>
	public int ItemSlot { get; set; }

	/// <summary>Type of enhancement being performed.</summary>
	public EnhancementType Type { get; set; }

	/// <summary>Inventory slot of protection item (if any).</summary>
	public int? ProtectionSlot { get; set; }

	/// <summary>Session start time.</summary>
	public DateTime StartedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Service for managing item enhancements.
/// </summary>
public class EnhancementService
{
	/// <summary>Maximum enhancement grade.</summary>
	public const int MaxGrade = 20;

	/// <summary>Safe enhancement levels (no failure penalty).</summary>
	private static readonly HashSet<int> SafeLevels = new() { 1, 2, 3, 4, 5 };

	/// <summary>Level configs by grade.</summary>
	private readonly Dictionary<int, EnhancementLevelConfig> _levelConfigs = new();

	/// <summary>Active enhancement sessions by player ID.</summary>
	private readonly ConcurrentDictionary<long, EnhancementSession> _activeSessions = new();

	/// <summary>Random for enhancement rolls.</summary>
	private readonly Random _random = new();

	/// <summary>
	/// Initializes the enhancement service with default level configs.
	/// </summary>
	public EnhancementService()
	{
		InitializeLevelConfigs();
	}

	/// <summary>
	/// Initializes level configurations with standard rates.
	/// </summary>
	private void InitializeLevelConfigs()
	{
		// Safe levels (1-5): High success, no penalties
		for (int i = 1; i <= 5; i++)
		{
			_levelConfigs[i] = new EnhancementLevelConfig
			{
				Grade = i,
				BaseSuccessRate = 100 - (i * 5), // 95% -> 75%
				DestructionChance = 0,
				DegradationChance = 0,
				IsSafeLevel = true,
				Cost = 1000 * i
			};
		}

		// Medium levels (6-10): Moderate success, degradation possible
		for (int i = 6; i <= 10; i++)
		{
			_levelConfigs[i] = new EnhancementLevelConfig
			{
				Grade = i,
				BaseSuccessRate = 80 - ((i - 5) * 8), // 72% -> 40%
				DestructionChance = 0,
				DegradationChance = 20 + ((i - 5) * 5), // 25% -> 45%
				DegradationAmount = 1,
				IsSafeLevel = false,
				Cost = 5000 * (i - 4)
			};
		}

		// High levels (11-15): Low success, destruction possible
		for (int i = 11; i <= 15; i++)
		{
			_levelConfigs[i] = new EnhancementLevelConfig
			{
				Grade = i,
				BaseSuccessRate = 40 - ((i - 10) * 5), // 35% -> 15%
				DestructionChance = 10 + ((i - 10) * 5), // 15% -> 35%
				DegradationChance = 30,
				DegradationAmount = 2,
				IsSafeLevel = false,
				Cost = 25000 * (i - 9)
			};
		}

		// Extreme levels (16-20): Very low success, high risk
		for (int i = 16; i <= 20; i++)
		{
			_levelConfigs[i] = new EnhancementLevelConfig
			{
				Grade = i,
				BaseSuccessRate = 15 - ((i - 15) * 2), // 13% -> 5%
				DestructionChance = 40 + ((i - 15) * 8), // 48% -> 80%
				DegradationChance = 30,
				DegradationAmount = 3,
				IsSafeLevel = false,
				Cost = 100000 * (i - 14)
			};
		}
	}

	/// <summary>
	/// Gets the level configuration for a grade.
	/// </summary>
	public EnhancementLevelConfig GetLevelConfig(int grade)
	{
		if (_levelConfigs.TryGetValue(grade, out var config))
		{
			return config;
		}

		// Return max level config for anything above
		return _levelConfigs[MaxGrade];
	}

	/// <summary>
	/// Validates if an item can be enhanced.
	/// </summary>
	public (bool CanEnhance, string ErrorMessage) CanEnhance(ItemModel item)
	{
		if (item.MaxEnchantCount <= 0)
		{
			return (false, "This item cannot be enhanced.");
		}

		if (item.Grade >= MaxGrade)
		{
			return (false, $"This item is already at maximum enhancement (+{MaxGrade}).");
		}

		if (item.Grade >= item.MaxEnchantCount)
		{
			return (false, $"This item has reached its enhancement limit (+{item.MaxEnchantCount}).");
		}

		if (item.IsBroken())
		{
			return (false, "This item is broken and must be repaired first.");
		}

		return (true, string.Empty);
	}

	/// <summary>
	/// Starts an enhancement session.
	/// </summary>
	public bool StartEnhancement(PlayerClient pc, int itemSlot, EnhancementType type, int? protectionSlot = null)
	{
		long playerId = pc.GetId();
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		// Check for existing session
		if (_activeSessions.ContainsKey(playerId))
		{
			Logger.Log($"[ENHANCE] {playerName} already has an active enhancement session", LogLevel.Warning);
			return false;
		}

		var inventory = pc.GetInventory();
		var item = inventory.AtSlot(itemSlot) as ItemModel;

		if (item == null)
		{
			Logger.Log($"[ENHANCE] {playerName} start failed: No item at slot {itemSlot}", LogLevel.Warning);
			return false;
		}

		var (canEnhance, errorMessage) = CanEnhance(item);
		if (!canEnhance)
		{
			Logger.Log($"[ENHANCE] {playerName} start failed: {errorMessage}", LogLevel.Warning);
			return false;
		}

		// Validate protection item if specified
		if (protectionSlot.HasValue)
		{
			var protectionItem = inventory.AtSlot(protectionSlot.Value) as ItemModel;
			if (protectionItem == null || !IsProtectionItem(protectionItem))
			{
				Logger.Log($"[ENHANCE] {playerName} start failed: Invalid protection item", LogLevel.Warning);
				return false;
			}
		}

		var session = new EnhancementSession
		{
			ItemSlot = itemSlot,
			Type = type,
			ProtectionSlot = protectionSlot
		};

		_activeSessions[playerId] = session;
		Logger.Log($"[ENHANCE] {playerName} started {type} enhancement on {item.Name} (+{item.Grade})", LogLevel.Debug);

		return true;
	}

	/// <summary>
	/// Cancels an active enhancement session.
	/// </summary>
	public void CancelEnhancement(PlayerClient pc)
	{
		long playerId = pc.GetId();
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		if (_activeSessions.TryRemove(playerId, out _))
		{
			Logger.Log($"[ENHANCE] {playerName} canceled enhancement", LogLevel.Debug);
		}
	}

	/// <summary>
	/// Executes the enhancement attempt.
	/// </summary>
	public EnhancementResult ExecuteEnhancement(PlayerClient pc, int[] materialSlots)
	{
		long playerId = pc.GetId();
		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		// Get session
		if (!_activeSessions.TryGetValue(playerId, out var session))
		{
			return EnhancementResult.Error("No active enhancement session.");
		}

		var inventory = pc.GetInventory();
		var item = inventory.AtSlot(session.ItemSlot) as ItemModel;

		if (item == null)
		{
			_activeSessions.TryRemove(playerId, out _);
			return EnhancementResult.Error("Item no longer exists.");
		}

		// Calculate costs and rates
		int currentGrade = item.Grade;
		var config = GetLevelConfig(currentGrade + 1);

		// Check cost
		if (inventory.WalletPeder < config.Cost)
		{
			return EnhancementResult.Error($"Insufficient funds. Need {config.Cost} Peder.");
		}

		// Collect materials and calculate bonus
		int materialBonus = 0;
		var materials = new List<(int slot, ItemModel item)>();

		if (materialSlots != null)
		{
			foreach (int matSlot in materialSlots)
			{
				if (matSlot > 0)
				{
					var material = inventory.AtSlot(matSlot) as ItemModel;
					if (material != null)
					{
						materials.Add((matSlot, material));
						materialBonus += GetMaterialBonus(material);
					}
				}
			}
		}

		// Calculate final success rate
		int baseRate = config.BaseSuccessRate;
		int typeBonus = GetTypeBonus(session.Type);
		int finalRate = Math.Min(100, Math.Max(1, baseRate + materialBonus + typeBonus));

		// Deduct cost
		inventory.WalletPeder -= config.Cost;

		// Consume materials
		foreach (var (matSlot, material) in materials)
		{
			if (material.Stackable() && material.Count > 1)
			{
				material.UpdateAmount(material.Count - 1);
				inventory.UpdateItemAtSlot(matSlot, material);
			}
			else
			{
				inventory.RemoveItem(matSlot);
			}
		}

		// Consume protection item if used
		if (session.ProtectionSlot.HasValue)
		{
			var protItem = inventory.AtSlot(session.ProtectionSlot.Value) as ItemModel;
			if (protItem != null)
			{
				if (protItem.Stackable() && protItem.Count > 1)
				{
					protItem.UpdateAmount(protItem.Count - 1);
					inventory.UpdateItemAtSlot(session.ProtectionSlot.Value, protItem);
				}
				else
				{
					inventory.RemoveItem(session.ProtectionSlot.Value);
				}
			}
		}

		// Roll for success
		int roll = _random.Next(100);
		EnhancementResult result;

		if (roll < finalRate)
		{
			// Success!
			item.Grade++;
			inventory.UpdateItemAtSlot(session.ItemSlot, item);

			result = EnhancementResult.Succeeded(currentGrade, item.Grade, finalRate, roll);
			Logger.Log($"[ENHANCE] {playerName} SUCCESS: {item.Name} +{currentGrade} -> +{item.Grade} (roll: {roll}, needed: {finalRate})", LogLevel.Information);
		}
		else
		{
			// Failure - determine consequences
			bool hasProtection = session.ProtectionSlot.HasValue || session.Type == EnhancementType.ProtectedUpgrade;
			bool isSafe = config.IsSafeLevel || session.Type == EnhancementType.SafeUpgrade;

			if (isSafe)
			{
				// Safe enhancement - no penalties
				result = EnhancementResult.FailedNoChange(currentGrade, finalRate, roll);
				Logger.Log($"[ENHANCE] {playerName} FAIL (safe): {item.Name} +{currentGrade} (roll: {roll}, needed: {finalRate})", LogLevel.Information);
			}
			else
			{
				// Check for destruction
				int destructionRoll = _random.Next(100);
				if (!hasProtection && destructionRoll < config.DestructionChance)
				{
					// Item destroyed
					inventory.RemoveItem(session.ItemSlot);
					result = EnhancementResult.FailedDestroyed(currentGrade, finalRate, roll);
					Logger.Log($"[ENHANCE] {playerName} DESTROYED: {item.Name} +{currentGrade} (roll: {roll}, destruction: {destructionRoll})", LogLevel.Warning);
				}
				else
				{
					// Check for degradation
					int degradationRoll = _random.Next(100);
					if (!hasProtection && degradationRoll < config.DegradationChance)
					{
						// Grade decreased
						int newGrade = Math.Max(0, currentGrade - config.DegradationAmount);
						item.Grade = newGrade;
						inventory.UpdateItemAtSlot(session.ItemSlot, item);

						result = EnhancementResult.FailedDegraded(currentGrade, newGrade, finalRate, roll);
						Logger.Log($"[ENHANCE] {playerName} DEGRADED: {item.Name} +{currentGrade} -> +{newGrade} (roll: {roll})", LogLevel.Information);
					}
					else
					{
						// No change
						result = EnhancementResult.FailedNoChange(currentGrade, finalRate, roll);
						Logger.Log($"[ENHANCE] {playerName} FAIL: {item.Name} +{currentGrade} (roll: {roll}, needed: {finalRate})", LogLevel.Information);
					}
				}
			}
		}

		// Clean up session
		_activeSessions.TryRemove(playerId, out _);

		// Update client
		pc.SendInventory();

		return result;
	}

	/// <summary>
	/// Checks if an item is a protection scroll/item.
	/// </summary>
	private bool IsProtectionItem(ItemModel item)
	{
		// Protection items typically have specific StuffType values
		return item.StuffType == 60 || item.StuffType == 61;
	}

	/// <summary>
	/// Gets the success rate bonus from a material.
	/// </summary>
	private int GetMaterialBonus(ItemModel material)
	{
		// Different material types provide different bonuses
		return material.StuffType switch
		{
			55 => 5,  // Basic enhancement stone
			56 => 10, // Advanced enhancement stone
			57 => 15, // Superior enhancement stone
			58 => 20, // Blessed enhancement stone
			_ => 2    // Other materials
		};
	}

	/// <summary>
	/// Gets the success rate bonus from enhancement type.
	/// </summary>
	private int GetTypeBonus(EnhancementType type)
	{
		return type switch
		{
			EnhancementType.BlessedUpgrade => 15,
			EnhancementType.SafeUpgrade => 0,
			EnhancementType.ProtectedUpgrade => 0,
			_ => 0
		};
	}

	/// <summary>
	/// Gets enhancement preview information.
	/// </summary>
	public EnhancementPreview GetEnhancementPreview(ItemModel item, EnhancementType type, int materialCount)
	{
		var config = GetLevelConfig(item.Grade + 1);
		int baseRate = config.BaseSuccessRate;
		int typeBonus = GetTypeBonus(type);
		int materialBonus = materialCount * 5; // Estimate

		return new EnhancementPreview
		{
			CurrentGrade = item.Grade,
			TargetGrade = item.Grade + 1,
			BaseSuccessRate = baseRate,
			FinalSuccessRate = Math.Min(100, baseRate + typeBonus + materialBonus),
			DestructionChance = config.DestructionChance,
			DegradationChance = config.DegradationChance,
			Cost = config.Cost,
			IsSafeLevel = config.IsSafeLevel
		};
	}

	/// <summary>
	/// Cleans up enhancement state for a player.
	/// </summary>
	public void CleanupPlayerEnhancement(long playerId)
	{
		_activeSessions.TryRemove(playerId, out _);
	}
}

/// <summary>
/// Preview information for an enhancement attempt.
/// </summary>
public class EnhancementPreview
{
	/// <summary>Current item grade.</summary>
	public int CurrentGrade { get; set; }

	/// <summary>Target grade if successful.</summary>
	public int TargetGrade { get; set; }

	/// <summary>Base success rate.</summary>
	public int BaseSuccessRate { get; set; }

	/// <summary>Final success rate with bonuses.</summary>
	public int FinalSuccessRate { get; set; }

	/// <summary>Chance of item destruction on failure.</summary>
	public int DestructionChance { get; set; }

	/// <summary>Chance of grade degradation on failure.</summary>
	public int DegradationChance { get; set; }

	/// <summary>Enhancement cost in Peder.</summary>
	public int Cost { get; set; }

	/// <summary>Whether this is a safe enhancement level.</summary>
	public bool IsSafeLevel { get; set; }
}
