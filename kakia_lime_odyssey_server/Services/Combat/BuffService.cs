using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Interfaces;
using kakia_lime_odyssey_server.Models;
using kakia_lime_odyssey_server.Models.BuffXML;
using kakia_lime_odyssey_server.Models.SkillXML;

namespace kakia_lime_odyssey_server.Services.Combat;

/// <summary>
/// Service for managing buffs, debuffs, and status effects on entities.
/// </summary>
public class BuffService : IBuffService
{
	// Entity ID -> List of active buffs
	private readonly ConcurrentDictionary<long, List<ActiveBuff>> _entityBuffs = new();

	// Global buff instance ID counter
	private static uint _nextBuffInstId = 1;
	private static readonly object _instIdLock = new();

	// Maximum buffs per entity (prevents memory issues)
	private const int MaxBuffsPerEntity = 32;

	// Buff type classifications
	private static readonly HashSet<int> DebuffTypes = new() { 2, 3, 4 }; // Based on buffType values

	/// <inheritdoc/>
	public BuffResult ApplyBuff(IEntity target, int buffTypeId, int level, int durationMs, IEntity? caster = null)
	{
		// Get buff data from XML
		var buffData = BuffInfo.GetBuff(buffTypeId);
		if (buffData == null)
		{
			Logger.Log($"[BUFF] Buff type {buffTypeId} not found in BuffInfo", LogLevel.Warning);
			return new BuffResult { Success = false, FailReason = BuffFailReason.BuffNotFound };
		}

		// Validate level
		if (level < 1)
		{
			return new BuffResult { Success = false, FailReason = BuffFailReason.InvalidLevel };
		}

		// Check if target is dead
		var targetStatus = target.GetEntityStatus();
		if (targetStatus.BasicStatus.Hp <= 0)
		{
			return new BuffResult { Success = false, FailReason = BuffFailReason.TargetDead };
		}

		long targetId = target.GetId();
		var buffs = _entityBuffs.GetOrAdd(targetId, _ => new List<ActiveBuff>());

		bool replacedExisting = false;

		lock (buffs)
		{
			// Check for existing buff of same type
			var existingBuff = buffs.FirstOrDefault(b => b.TypeId == buffTypeId);
			if (existingBuff != null)
			{
				// Replace existing buff (refresh duration, update level)
				existingBuff.Level = level;
				existingBuff.RemainingDurationMs = durationMs;
				existingBuff.TotalDurationMs = durationMs;
				existingBuff.AppliedAt = DateTime.UtcNow;
				existingBuff.CasterId = caster?.GetId() ?? 0;
				existingBuff.Modifiers = ParseBuffModifiers(buffData, level);

				var updatePacket = BuildInsertDefPacket(existingBuff, targetId);

				Logger.Log($"[BUFF] Refreshed buff {buffData.Name} (ID:{buffTypeId}) on entity {targetId}, duration: {durationMs}ms", LogLevel.Debug);

				return new BuffResult
				{
					Success = true,
					BuffInstId = existingBuff.InstId,
					ReplacedExisting = true,
					Packet = updatePacket
				};
			}

			// Check max buffs limit
			if (buffs.Count >= MaxBuffsPerEntity)
			{
				return new BuffResult { Success = false, FailReason = BuffFailReason.MaxBuffsReached };
			}

			// Create new buff instance
			uint instId;
			lock (_instIdLock)
			{
				instId = _nextBuffInstId++;
			}

			var newBuff = new ActiveBuff
			{
				InstId = instId,
				TypeId = buffTypeId,
				Level = level,
				RemainingDurationMs = durationMs,
				TotalDurationMs = durationMs,
				AppliedAt = DateTime.UtcNow,
				CasterId = caster?.GetId() ?? 0,
				IsDebuff = DebuffTypes.Contains(buffData.BuffType),
				IsStun = buffData.Stun == 1,
				Name = buffData.Name,
				Modifiers = ParseBuffModifiers(buffData, level)
			};

			buffs.Add(newBuff);

			var packet = BuildInsertDefPacket(newBuff, targetId);

			Logger.Log($"[BUFF] Applied buff {buffData.Name} (ID:{buffTypeId}, InstID:{instId}) to entity {targetId}, duration: {durationMs}ms", LogLevel.Debug);

			return new BuffResult
			{
				Success = true,
				BuffInstId = instId,
				ReplacedExisting = false,
				Packet = packet
			};
		}
	}

	/// <inheritdoc/>
	public bool RemoveBuff(IEntity target, uint buffInstId)
	{
		long targetId = target.GetId();

		if (!_entityBuffs.TryGetValue(targetId, out var buffs))
			return false;

		lock (buffs)
		{
			var buff = buffs.FirstOrDefault(b => b.InstId == buffInstId);
			if (buff == null)
				return false;

			buffs.Remove(buff);

			Logger.Log($"[BUFF] Removed buff {buff.Name} (InstID:{buffInstId}) from entity {targetId}", LogLevel.Debug);

			return true;
		}
	}

	/// <inheritdoc/>
	public int RemoveBuffsByType(IEntity target, int buffTypeId)
	{
		long targetId = target.GetId();

		if (!_entityBuffs.TryGetValue(targetId, out var buffs))
			return 0;

		lock (buffs)
		{
			int removed = buffs.RemoveAll(b => b.TypeId == buffTypeId);

			if (removed > 0)
				Logger.Log($"[BUFF] Removed {removed} buffs of type {buffTypeId} from entity {targetId}", LogLevel.Debug);

			return removed;
		}
	}

	/// <inheritdoc/>
	public int ClearAllBuffs(IEntity target, bool includeDebuffs = true)
	{
		long targetId = target.GetId();

		if (!_entityBuffs.TryGetValue(targetId, out var buffs))
			return 0;

		lock (buffs)
		{
			int removed;
			if (includeDebuffs)
			{
				removed = buffs.Count;
				buffs.Clear();
			}
			else
			{
				removed = buffs.RemoveAll(b => !b.IsDebuff);
			}

			if (removed > 0)
				Logger.Log($"[BUFF] Cleared {removed} buffs from entity {targetId}", LogLevel.Debug);

			return removed;
		}
	}

	/// <inheritdoc/>
	public IReadOnlyList<ActiveBuff> GetActiveBuffs(IEntity target)
	{
		long targetId = target.GetId();

		if (!_entityBuffs.TryGetValue(targetId, out var buffs))
			return Array.Empty<ActiveBuff>();

		lock (buffs)
		{
			return buffs.ToList().AsReadOnly();
		}
	}

	/// <inheritdoc/>
	public bool HasBuff(IEntity target, int buffTypeId)
	{
		long targetId = target.GetId();

		if (!_entityBuffs.TryGetValue(targetId, out var buffs))
			return false;

		lock (buffs)
		{
			return buffs.Any(b => b.TypeId == buffTypeId);
		}
	}

	/// <inheritdoc/>
	public List<(IEntity Entity, ActiveBuff Buff)> UpdateBuffTimers(int deltaMs)
	{
		var expiredBuffs = new List<(long entityId, ActiveBuff buff)>();
		var result = new List<(IEntity Entity, ActiveBuff Buff)>();

		foreach (var kvp in _entityBuffs)
		{
			long entityId = kvp.Key;
			var buffs = kvp.Value;

			lock (buffs)
			{
				foreach (var buff in buffs)
				{
					// Skip permanent buffs (duration 0)
					if (buff.TotalDurationMs == 0)
						continue;

					buff.RemainingDurationMs -= deltaMs;

					if (buff.RemainingDurationMs <= 0)
					{
						expiredBuffs.Add((entityId, buff));
					}
				}

				// Remove expired buffs and add to result
				foreach (var (expEntityId, expiredBuff) in expiredBuffs.Where(e => e.entityId == entityId))
				{
					buffs.Remove(expiredBuff);
					Logger.Log($"[BUFF] Expired buff {expiredBuff.Name} (InstID:{expiredBuff.InstId}) on entity {expEntityId}", LogLevel.Debug);

					// Try to resolve entity for result
					if (Network.LimeServer.TryGetEntity(expEntityId, out var entity) && entity != null)
					{
						result.Add((entity, expiredBuff));
					}
				}
			}
		}

		return result;
	}

	/// <inheritdoc/>
	public BuffStatModifiers GetBuffModifiers(IEntity target)
	{
		long targetId = target.GetId();
		var result = new BuffStatModifiers();

		if (!_entityBuffs.TryGetValue(targetId, out var buffs))
			return result;

		lock (buffs)
		{
			foreach (var buff in buffs)
			{
				result.Add(buff.Modifiers);
			}
		}

		return result;
	}

	/// <summary>
	/// Builds a DEF packet list for login/zone entry.
	/// </summary>
	public byte[] BuildDefListPacket(IEntity target)
	{
		var buffs = GetActiveBuffs(target);

		using PacketWriter pw = new();

		SC_DEF_LIST defList = new()
		{
			count = (ushort)buffs.Count
		};

		pw.Write(defList);

		foreach (var buff in buffs)
		{
			DEF def = new()
			{
				instID = buff.InstId,
				typeID = (uint)buff.TypeId,
				lv = (ushort)buff.Level,
				durTime = buff.RemainingDurationMs
			};
			pw.Write(def);
		}

		return pw.ToSizedPacket();
	}

	/// <summary>
	/// Builds a remove DEF packet.
	/// </summary>
	public static byte[] BuildRemoveDefPacket(long entityId, uint buffInstId)
	{
		using PacketWriter pw = new();

		SC_REMOVE_DEF removeDef = new()
		{
			objInstID = entityId,
			instID = buffInstId
		};

		pw.Write(removeDef);
		return pw.ToPacket();
	}

	/// <summary>
	/// Builds a remove DEF packet for a given buff instance ID.
	/// Uses entity ID 0 which signals self-removal to the client.
	/// </summary>
	public static byte[] BuildRemoveDefPacket(uint buffInstId)
	{
		return BuildRemoveDefPacket(0, buffInstId);
	}

	/// <summary>
	/// Parses buff modifiers from XML data based on buff level.
	/// </summary>
	private static BuffStatModifiers ParseBuffModifiers(XmlBuff buffData, int level)
	{
		var modifiers = new BuffStatModifiers();

		// Check for stun effect
		if (buffData.Stun == 1)
		{
			modifiers.IsStunned = true;
		}

		// Parse buff subject list for level-specific effects
		if (buffData.Subject?.SubjectLists != null)
		{
			var subjectData = buffData.Subject.SubjectLists
				.FirstOrDefault(s => s.SubjectLevel == level);

			if (subjectData != null && !string.IsNullOrEmpty(subjectData.Detail))
			{
				ParseDetailString(subjectData.Detail, modifiers);
			}
		}

		// Parse buffType for general effect category
		switch (buffData.BuffType)
		{
			case 1: // Positive buff
				break;
			case 2: // Movement debuff
				modifiers.MovementSpeedPercent -= 20;
				modifiers.IsRooted = buffData.Name.Contains("Root", StringComparison.OrdinalIgnoreCase);
				break;
			case 3: // Silence debuff
				modifiers.IsSilenced = true;
				break;
			case 4: // Damage debuff
				modifiers.MeleeDefPercent -= 10;
				modifiers.SpellDefPercent -= 10;
				break;
		}

		return modifiers;
	}

	/// <summary>
	/// Parses the detail string for stat modifiers.
	/// Format examples: "STR+10", "ATK+5%", "DEF-20"
	/// </summary>
	private static void ParseDetailString(string detail, BuffStatModifiers modifiers)
	{
		if (string.IsNullOrWhiteSpace(detail))
			return;

		// Split by common delimiters
		var parts = detail.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);

		foreach (var part in parts)
		{
			var trimmed = part.Trim();

			// Try to parse stat modifiers like "STR+10" or "ATK+5%"
			bool isPercent = trimmed.EndsWith('%');
			if (isPercent)
				trimmed = trimmed.TrimEnd('%');

			int value = 0;
			string stat = string.Empty;

			if (trimmed.Contains('+'))
			{
				var split = trimmed.Split('+');
				stat = split[0].ToUpperInvariant();
				int.TryParse(split[1], out value);
			}
			else if (trimmed.Contains('-'))
			{
				var split = trimmed.Split('-');
				stat = split[0].ToUpperInvariant();
				int.TryParse(split[1], out value);
				value = -value;
			}

			ApplyStatModifier(modifiers, stat, value, isPercent);
		}
	}

	/// <summary>
	/// Applies a parsed stat modifier to the modifiers object.
	/// </summary>
	private static void ApplyStatModifier(BuffStatModifiers modifiers, string stat, int value, bool isPercent)
	{
		switch (stat)
		{
			case "STR":
				modifiers.StrengthFlat += value;
				break;
			case "INT":
				modifiers.IntelligenceFlat += value;
				break;
			case "DEX":
				modifiers.DexterityFlat += value;
				break;
			case "AGI":
				modifiers.AgilityFlat += value;
				break;
			case "VIT":
				modifiers.VitalityFlat += value;
				break;
			case "SPI":
				modifiers.SpiritFlat += value;
				break;
			case "LUK":
				modifiers.LuckyFlat += value;
				break;
			case "ATK":
			case "MELEEATK":
				if (isPercent)
					modifiers.MeleeAtkPercent += value;
				else
					modifiers.MeleeAtkFlat += value;
				break;
			case "DEF":
			case "MELEEDEF":
				if (isPercent)
					modifiers.MeleeDefPercent += value;
				else
					modifiers.MeleeDefFlat += value;
				break;
			case "SPELLATK":
			case "MATK":
				if (isPercent)
					modifiers.SpellAtkPercent += value;
				else
					modifiers.SpellAtkFlat += value;
				break;
			case "SPELLDEF":
			case "MDEF":
				if (isPercent)
					modifiers.SpellDefPercent += value;
				else
					modifiers.SpellDefFlat += value;
				break;
			case "HIT":
				modifiers.HitRateFlat += value;
				break;
			case "FLEE":
			case "DODGE":
				modifiers.FleeRateFlat += value;
				break;
			case "CRIT":
				modifiers.CritRateFlat += value;
				break;
			case "HP":
			case "MAXHP":
				modifiers.MaxHpFlat += value;
				break;
			case "MP":
			case "MAXMP":
				modifiers.MaxMpFlat += value;
				break;
			case "MOVESPEED":
			case "SPEED":
				modifiers.MovementSpeedPercent += value;
				break;
			case "ASPD":
			case "ATTACKSPEED":
				modifiers.AttackSpeedPercent += value;
				break;
		}
	}

	/// <summary>
	/// Builds an insert DEF packet for buff application.
	/// </summary>
	private static byte[] BuildInsertDefPacket(ActiveBuff buff, long targetId)
	{
		using PacketWriter pw = new();

		SC_INSERT_DEF insertDef = new()
		{
			def = new DEF
			{
				instID = buff.InstId,
				typeID = (uint)buff.TypeId,
				lv = (ushort)buff.Level,
				durTime = buff.RemainingDurationMs
			},
			objInstID = targetId
		};

		pw.Write(insertDef);
		return pw.ToPacket();
	}

	/// <summary>
	/// Applies buffs from a skill to a target entity.
	/// Parses the skill's detail string and nested subjects for buff references.
	/// </summary>
	/// <param name="skill">The skill being used</param>
	/// <param name="skillLevel">The level of the skill</param>
	/// <param name="target">Entity receiving the buff(s)</param>
	/// <param name="caster">Entity casting the skill (optional)</param>
	/// <returns>List of buff results for each buff applied</returns>
	public List<BuffResult> ApplyBuffsFromSkill(XmlSkill skill, int skillLevel, IEntity target, IEntity? caster = null)
	{
		var results = new List<BuffResult>();

		if (skill?.Subject?.SubjectLists == null)
			return results;

		// Find the subject list for this skill level
		var subjectData = skill.Subject.SubjectLists
			.FirstOrDefault(s => s.SubjectLevel == skillLevel)
			?? skill.Subject.SubjectLists.FirstOrDefault();

		if (subjectData == null)
			return results;

		// Parse buff references from detail string
		var buffRefs = ParseBuffReferencesFromDetail(subjectData.Detail);

		// Also check nested subjects for buff skill IDs
		if (subjectData.Subjects != null)
		{
			foreach (var subject in subjectData.Subjects)
			{
				// Skill IDs can reference buffs directly
				var buff = BuffInfo.GetBuff(subject.SkillId);
				if (buff != null)
				{
					buffRefs.Add(new SkillBuffReference
					{
						BuffId = subject.SkillId,
						Level = subject.Level > 0 ? subject.Level : skillLevel,
						DurationMs = GetDefaultBuffDuration(buff)
					});
				}
			}
		}

		// Apply each referenced buff
		foreach (var buffRef in buffRefs)
		{
			var result = ApplyBuff(target, buffRef.BuffId, buffRef.Level, buffRef.DurationMs, caster);
			results.Add(result);
		}

		return results;
	}

	/// <summary>
	/// Checks if a skill type is a buff-applying skill.
	/// </summary>
	public static bool IsBuffSkill(string skillType)
	{
		if (string.IsNullOrEmpty(skillType))
			return false;

		var buffTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"Buff",
			"SelfBuff",
			"PartyBuff",
			"TargetBuff",
			"Aura",
			"Shield",
			"Barrier",
			"Enhancement",
			"Blessing",
			"Protection"
		};

		return buffTypes.Contains(skillType) ||
		       skillType.Contains("Buff", StringComparison.OrdinalIgnoreCase) ||
		       skillType.Contains("Aura", StringComparison.OrdinalIgnoreCase);
	}

	/// <summary>
	/// Checks if a skill type is a debuff-applying skill.
	/// </summary>
	public static bool IsDebuffSkill(string skillType)
	{
		if (string.IsNullOrEmpty(skillType))
			return false;

		var debuffTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"Debuff",
			"Curse",
			"Poison",
			"Slow",
			"Stun",
			"Silence",
			"Root",
			"Fear",
			"Sleep",
			"Weaken"
		};

		return debuffTypes.Contains(skillType) ||
		       skillType.Contains("Debuff", StringComparison.OrdinalIgnoreCase) ||
		       skillType.Contains("Curse", StringComparison.OrdinalIgnoreCase);
	}

	/// <summary>
	/// Parses buff references from a skill detail string.
	/// Looks for patterns like "buff:123", "buffId=123", "applyBuff(123, 5000)", etc.
	/// </summary>
	private static List<SkillBuffReference> ParseBuffReferencesFromDetail(string detail)
	{
		var refs = new List<SkillBuffReference>();

		if (string.IsNullOrWhiteSpace(detail))
			return refs;

		// Pattern: buff:ID or buffId=ID
		var buffIdPattern = new Regex(@"buff(?:Id)?[=:]?\s*(\d+)", RegexOptions.IgnoreCase);
		foreach (Match match in buffIdPattern.Matches(detail))
		{
			if (int.TryParse(match.Groups[1].Value, out int buffId))
			{
				refs.Add(new SkillBuffReference
				{
					BuffId = buffId,
					Level = 1,
					DurationMs = 30000 // Default 30 seconds
				});
			}
		}

		// Pattern: applyBuff(buffId, duration) or applybuff(buffId, duration, level)
		var applyBuffPattern = new Regex(@"applyBuff\s*\(\s*(\d+)\s*(?:,\s*(\d+))?\s*(?:,\s*(\d+))?\s*\)", RegexOptions.IgnoreCase);
		foreach (Match match in applyBuffPattern.Matches(detail))
		{
			if (int.TryParse(match.Groups[1].Value, out int buffId))
			{
				int duration = match.Groups[2].Success && int.TryParse(match.Groups[2].Value, out int d) ? d : 30000;
				int level = match.Groups[3].Success && int.TryParse(match.Groups[3].Value, out int l) ? l : 1;

				refs.Add(new SkillBuffReference
				{
					BuffId = buffId,
					Level = level,
					DurationMs = duration
				});
			}
		}

		// Pattern: effect:buffName or effectType=buffName (lookup by name)
		var effectNamePattern = new Regex(@"effect(?:Type)?[=:]\s*(\w+)", RegexOptions.IgnoreCase);
		foreach (Match match in effectNamePattern.Matches(detail))
		{
			string effectName = match.Groups[1].Value;
			var buff = BuffInfo.GetBuffByName(effectName);
			if (buff != null)
			{
				refs.Add(new SkillBuffReference
				{
					BuffId = buff.Id,
					Level = 1,
					DurationMs = GetDefaultBuffDuration(buff)
				});
			}
		}

		return refs;
	}

	/// <summary>
	/// Gets a default buff duration based on buff type.
	/// </summary>
	private static int GetDefaultBuffDuration(XmlBuff buff)
	{
		// Debuffs typically shorter, buffs longer
		return buff.BuffType switch
		{
			1 => 60000, // Positive buff: 60 seconds
			2 => 15000, // Movement debuff: 15 seconds
			3 => 10000, // Silence: 10 seconds
			4 => 20000, // Damage debuff: 20 seconds
			_ => 30000  // Default: 30 seconds
		};
	}
}

/// <summary>
/// Reference to a buff that should be applied from a skill.
/// </summary>
public class SkillBuffReference
{
	public int BuffId { get; set; }
	public int Level { get; set; }
	public int DurationMs { get; set; }
}
