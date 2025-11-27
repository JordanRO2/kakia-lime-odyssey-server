/// <summary>
/// Service for managing player skill learning and progression.
/// </summary>
/// <remarks>
/// Handles skill learning from NPCs, skill point distribution, and skill level ups.
/// Uses: MongoDBService for skill persistence, SkillDB for skill definitions
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Database;
using kakia_lime_odyssey_server.Models.Persistence;
using kakia_lime_odyssey_server.Models.SkillXML;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.Services.Skill;

/// <summary>
/// Service for managing player skill learning and progression.
/// </summary>
public class SkillService
{
	/// <summary>
	/// Attempts to learn a new skill for a player.
	/// </summary>
	/// <param name="pc">The player client requesting to learn</param>
	/// <param name="skillTypeId">Skill type ID from SkillInfo.xml</param>
	/// <param name="level">Requested skill level</param>
	public void LearnSkill(PlayerClient pc, uint skillTypeId, int level)
	{
		var character = pc.GetCurrentCharacter();
		if (character == null)
		{
			Logger.Log("[SKILL] LearnSkill failed: No character loaded", LogLevel.Warning);
			return;
		}

		// Validate skill exists in skill database
		var skillDef = LimeServer.SkillDB.FirstOrDefault(s => s.Id == (int)skillTypeId);
		if (skillDef == null)
		{
			Logger.Log($"[SKILL] LearnSkill failed: Invalid skill ID {skillTypeId}", LogLevel.Warning);
			return;
		}

		// Get player's current skills
		var accountId = pc.GetAccountId();
		var charName = character.appearance.name;
		var playerSkills = MongoDBService.Instance.GetPlayerSkills(accountId, charName);

		// Check if already learned
		var existingSkill = playerSkills.Skills.FirstOrDefault(s => s.SkillId == (int)skillTypeId);
		if (existingSkill != null)
		{
			Logger.Log($"[SKILL] {charName} already knows skill {skillDef.Name} ({skillTypeId})", LogLevel.Debug);
			// Could upgrade level instead
			return;
		}

		// Add the skill
		var newSkill = new LearnedSkill
		{
			SkillId = (int)skillTypeId,
			Level = (byte)Math.Max(1, level),
			Experience = 0,
			OnHotbar = false,
			HotbarSlot = 0
		};

		playerSkills.Skills.Add(newSkill);

		// Save to database
		MongoDBService.Instance.SavePlayerSkills(accountId, charName, playerSkills);

		Logger.Log($"[SKILL] {charName} learned skill {skillDef.Name} (ID: {skillTypeId}, Lv: {newSkill.Level})", LogLevel.Information);

		// Send SC_SKILL_ADD to client
		SendSkillAdd(pc, skillTypeId, newSkill.Level);
	}

	/// <summary>
	/// Distributes combat job skill points to level up a skill.
	/// </summary>
	/// <param name="pc">The player client</param>
	/// <param name="skillTypeId">Skill type ID to level up</param>
	/// <param name="points">Number of points to invest</param>
	public void DistributeSkillPoints(PlayerClient pc, uint skillTypeId, ushort points)
	{
		var character = pc.GetCurrentCharacter();
		if (character == null)
		{
			Logger.Log("[SKILL] DistributeSkillPoints failed: No character loaded", LogLevel.Warning);
			return;
		}

		// Validate skill exists
		var skillDef = LimeServer.SkillDB.FirstOrDefault(s => s.Id == (int)skillTypeId);
		if (skillDef == null)
		{
			Logger.Log($"[SKILL] DistributeSkillPoints failed: Invalid skill ID {skillTypeId}", LogLevel.Warning);
			return;
		}

		var accountId = pc.GetAccountId();
		var charName = character.appearance.name;
		var playerSkills = MongoDBService.Instance.GetPlayerSkills(accountId, charName);

		// Check if player has the skill
		var skill = playerSkills.Skills.FirstOrDefault(s => s.SkillId == (int)skillTypeId);
		if (skill == null)
		{
			Logger.Log($"[SKILL] {charName} doesn't have skill {skillTypeId} to level up", LogLevel.Warning);
			return;
		}

		// Check if player has enough skill points
		if (playerSkills.SkillPoints < points)
		{
			Logger.Log($"[SKILL] {charName} doesn't have enough skill points ({playerSkills.SkillPoints} < {points})", LogLevel.Warning);
			return;
		}

		// Level up the skill
		skill.Level += (byte)points;
		playerSkills.SkillPoints -= points;

		// Save to database
		MongoDBService.Instance.SavePlayerSkills(accountId, charName, playerSkills);

		Logger.Log($"[SKILL] {charName} leveled up {skillDef.Name} to Lv{skill.Level} (spent {points} points)", LogLevel.Information);

		// Send skill level up notification
		SendSkillLevelUp(pc, skillTypeId, skill.Level);
	}

	/// <summary>
	/// Sends SC_SKILL_ADD packet to notify client of new skill.
	/// </summary>
	private void SendSkillAdd(PlayerClient pc, uint skillTypeId, byte level)
	{
		var packet = new PACKET_SC_SKILL_ADD
		{
			header = new kakia_lime_odyssey_packets.Packets.Common.PACKET_FIX
			{
				packetType = (ushort)PacketType.SC_SKILL_ADD
			},
			typeID = skillTypeId,
			level = level
		};

		using PacketWriter pw = new();
		pw.Write(packet);
		pc.Send(pw.ToSizedPacket(), default).Wait();
	}

	/// <summary>
	/// Sends SC_SKILL_LV_UP packet to notify client of skill level increase.
	/// </summary>
	private void SendSkillLevelUp(PlayerClient pc, uint skillTypeId, byte newLevel)
	{
		// SC_SKILL_LV_UP is a variable-length packet
		// Format: PACKET_VAR header + skill data
		using PacketWriter pw = new();

		// Write packet type
		pw.Writer.Write((ushort)PacketType.SC_SKILL_LV_UP);

		// Write skill data
		pw.Writer.Write(skillTypeId);
		pw.Writer.Write(newLevel);

		pc.Send(pw.ToSizedPacket(), default).Wait();
	}

	/// <summary>
	/// Gets all learned skills for a character.
	/// </summary>
	/// <param name="pc">The player client</param>
	/// <returns>List of learned skills</returns>
	public List<LearnedSkill> GetLearnedSkills(PlayerClient pc)
	{
		var character = pc.GetCurrentCharacter();
		if (character == null)
			return new List<LearnedSkill>();

		var accountId = pc.GetAccountId();
		var charName = character.appearance.name;
		var playerSkills = MongoDBService.Instance.GetPlayerSkills(accountId, charName);

		return playerSkills.Skills;
	}

	/// <summary>
	/// Gets the skill definition from SkillDB.
	/// </summary>
	/// <param name="skillTypeId">Skill type ID</param>
	/// <returns>Skill definition or null if not found</returns>
	public XmlSkill? GetSkillDefinition(uint skillTypeId)
	{
		return LimeServer.SkillDB.FirstOrDefault(s => s.Id == (int)skillTypeId);
	}

	/// <summary>
	/// Checks if a player has learned a specific skill.
	/// </summary>
	/// <param name="pc">The player client</param>
	/// <param name="skillTypeId">Skill type ID to check</param>
	/// <returns>True if the player has the skill</returns>
	public bool HasSkill(PlayerClient pc, uint skillTypeId)
	{
		var skills = GetLearnedSkills(pc);
		return skills.Any(s => s.SkillId == (int)skillTypeId);
	}

	/// <summary>
	/// Gets the level of a specific skill for a player.
	/// </summary>
	/// <param name="pc">The player client</param>
	/// <param name="skillTypeId">Skill type ID</param>
	/// <returns>Skill level or 0 if not learned</returns>
	public byte GetSkillLevel(PlayerClient pc, uint skillTypeId)
	{
		var skills = GetLearnedSkills(pc);
		var skill = skills.FirstOrDefault(s => s.SkillId == (int)skillTypeId);
		return skill?.Level ?? 0;
	}

	/// <summary>
	/// Adds skill points to a player's available pool.
	/// </summary>
	/// <param name="pc">The player client</param>
	/// <param name="points">Number of points to add</param>
	public void AddSkillPoints(PlayerClient pc, int points)
	{
		var character = pc.GetCurrentCharacter();
		if (character == null)
			return;

		var accountId = pc.GetAccountId();
		var charName = character.appearance.name;
		var playerSkills = MongoDBService.Instance.GetPlayerSkills(accountId, charName);

		playerSkills.SkillPoints += points;

		MongoDBService.Instance.SavePlayerSkills(accountId, charName, playerSkills);

		Logger.Log($"[SKILL] {charName} gained {points} skill points (total: {playerSkills.SkillPoints})", LogLevel.Debug);
	}
}
