/// <summary>
/// Service for validating and consuming skill resource costs (HP/MP/SP/LP).
/// </summary>
/// <remarks>
/// Validates that players have sufficient resources before skill use.
/// Consumes resources upon successful validation.
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_server.Interfaces;
using kakia_lime_odyssey_server.Models;
using kakia_lime_odyssey_server.Models.SkillXML;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.Services.Combat;

/// <summary>
/// Result of a resource validation check.
/// </summary>
public class ResourceValidationResult
{
	/// <summary>Whether the validation passed.</summary>
	public bool IsValid { get; set; }

	/// <summary>Error message if validation failed.</summary>
	public string ErrorMessage { get; set; } = string.Empty;

	/// <summary>Type of resource that failed validation.</summary>
	public ResourceType FailedResource { get; set; } = ResourceType.None;

	/// <summary>Required amount of the resource.</summary>
	public int Required { get; set; }

	/// <summary>Current amount of the resource.</summary>
	public int Current { get; set; }

	/// <summary>Creates a success result.</summary>
	public static ResourceValidationResult Success() => new() { IsValid = true };

	/// <summary>Creates a failure result.</summary>
	public static ResourceValidationResult Failure(ResourceType resource, int required, int current, string message) =>
		new()
		{
			IsValid = false,
			FailedResource = resource,
			Required = required,
			Current = current,
			ErrorMessage = message
		};
}

/// <summary>
/// Types of resources that skills can consume.
/// </summary>
public enum ResourceType
{
	/// <summary>No resource.</summary>
	None,
	/// <summary>Health Points.</summary>
	HP,
	/// <summary>Mana Points.</summary>
	MP,
	/// <summary>Stamina Points (streamPoint).</summary>
	SP,
	/// <summary>Life Points (for life skills).</summary>
	LP
}

/// <summary>
/// Resource cost style types.
/// </summary>
public enum ResourceCostStyle
{
	/// <summary>Flat amount (e.g., 50 MP).</summary>
	Flat = 0,
	/// <summary>Percentage of max (e.g., 10% of max MP).</summary>
	Percentage = 1
}

/// <summary>
/// Service for validating and consuming skill resource costs.
/// </summary>
public static class SkillResourceValidator
{
	/// <summary>
	/// Validates that an entity has sufficient resources to use a skill.
	/// Does NOT consume resources - call ConsumeResources after successful validation.
	/// </summary>
	/// <param name="entity">The entity using the skill.</param>
	/// <param name="skill">The skill being used.</param>
	/// <param name="skillLevel">The level of the skill (1-based).</param>
	/// <returns>Validation result.</returns>
	public static ResourceValidationResult ValidateResources(IEntity entity, XmlSkill skill, int skillLevel = 1)
	{
		var skillData = GetSkillLevelData(skill, skillLevel);
		if (skillData == null)
		{
			return ResourceValidationResult.Success(); // No cost data, allow skill
		}

		var status = entity.GetEntityStatus();

		// Validate HP cost
		if (skillData.UseHP > 0)
		{
			int cost = CalculateCost(skillData.UseHP, skillData.UseHPStyle, (int)status.BasicStatus.Hp, (int)status.BasicStatus.MaxHp);
			if (status.BasicStatus.Hp < cost)
			{
				return ResourceValidationResult.Failure(
					ResourceType.HP, cost, (int)status.BasicStatus.Hp,
					$"Insufficient HP: requires {cost}, have {status.BasicStatus.Hp}");
			}
		}

		// Validate MP cost
		if (skillData.UseMP > 0)
		{
			int cost = CalculateCost(skillData.UseMP, skillData.UseMPStyle, (int)status.BasicStatus.Mp, (int)status.BasicStatus.MaxMp);
			if (status.BasicStatus.Mp < cost)
			{
				return ResourceValidationResult.Failure(
					ResourceType.MP, cost, (int)status.BasicStatus.Mp,
					$"Insufficient MP: requires {cost}, have {status.BasicStatus.Mp}");
			}
		}

		// Validate SP cost (streamPoint) - SP uses flat cost only since no MaxSP is defined
		if (skillData.UseSP > 0)
		{
			int cost = skillData.UseSP; // Flat cost only for SP
			if (status.StreamPoint < cost)
			{
				return ResourceValidationResult.Failure(
					ResourceType.SP, cost, (int)status.StreamPoint,
					$"Insufficient SP: requires {cost}, have {status.StreamPoint}");
			}
		}

		// Validate LP cost
		if (skillData.UseLP > 0)
		{
			int cost = CalculateCost(skillData.UseLP, skillData.UseLPStyle, (int)status.Lp, (int)status.MaxLp);
			if (status.Lp < cost)
			{
				return ResourceValidationResult.Failure(
					ResourceType.LP, cost, (int)status.Lp,
					$"Insufficient LP: requires {cost}, have {status.Lp}");
			}
		}

		return ResourceValidationResult.Success();
	}

	/// <summary>
	/// Consumes resources for a skill use from a PlayerClient.
	/// Should only be called after ValidateResources returns success.
	/// </summary>
	/// <param name="pc">The player client.</param>
	/// <param name="skill">The skill being used.</param>
	/// <param name="skillLevel">The level of the skill (1-based).</param>
	/// <returns>True if resources were consumed.</returns>
	public static bool ConsumeResources(PlayerClient pc, XmlSkill skill, int skillLevel = 1)
	{
		var skillData = GetSkillLevelData(skill, skillLevel);
		if (skillData == null)
		{
			return true; // No cost data, nothing to consume
		}

		var character = pc.GetCurrentCharacter();
		if (character == null)
		{
			return false;
		}

		var entityStatus = pc.GetEntityStatus();
		bool anyConsumed = false;

		// Consume HP
		if (skillData.UseHP > 0)
		{
			int cost = CalculateCost(skillData.UseHP, skillData.UseHPStyle, (int)entityStatus.BasicStatus.Hp, (int)entityStatus.BasicStatus.MaxHp);
			character.status.hp = (uint)Math.Max(0, (int)character.status.hp - cost);
			anyConsumed = true;
		}

		// Consume MP
		if (skillData.UseMP > 0)
		{
			int cost = CalculateCost(skillData.UseMP, skillData.UseMPStyle, (int)entityStatus.BasicStatus.Mp, (int)entityStatus.BasicStatus.MaxMp);
			character.status.mp = (uint)Math.Max(0, (int)character.status.mp - cost);
			anyConsumed = true;
		}

		// Consume SP (streamPoint) - flat cost only
		if (skillData.UseSP > 0)
		{
			int cost = skillData.UseSP;
			character.status.streamPoint = (uint)Math.Max(0, (int)character.status.streamPoint - cost);
			anyConsumed = true;
		}

		// Consume LP
		if (skillData.UseLP > 0)
		{
			int cost = CalculateCost(skillData.UseLP, skillData.UseLPStyle, (int)entityStatus.Lp, (int)entityStatus.MaxLp);
			character.status.lp = (uint)Math.Max(0, (int)character.status.lp - cost);
			anyConsumed = true;
		}

		if (anyConsumed)
		{
			string playerName = character.appearance.name ?? "Unknown";
			Logger.Log($"[SKILL RESOURCE] {playerName} consumed resources for skill {skill.Id} ({skill.NameEng})", LogLevel.Debug);
		}

		return true;
	}

	/// <summary>
	/// Validates resources and consumes them in one operation.
	/// </summary>
	/// <param name="pc">The player client.</param>
	/// <param name="skill">The skill being used.</param>
	/// <param name="skillLevel">The level of the skill (1-based).</param>
	/// <returns>Validation result. If valid, resources have been consumed.</returns>
	public static ResourceValidationResult ValidateAndConsume(PlayerClient pc, XmlSkill skill, int skillLevel = 1)
	{
		var validation = ValidateResources(pc, skill, skillLevel);
		if (!validation.IsValid)
		{
			return validation;
		}

		ConsumeResources(pc, skill, skillLevel);
		return validation;
	}

	/// <summary>
	/// Gets the resource costs for a skill at a specific level.
	/// </summary>
	/// <param name="skill">The skill.</param>
	/// <param name="skillLevel">The level (1-based).</param>
	/// <returns>Skill costs, or null if not found.</returns>
	public static SkillResourceCosts? GetResourceCosts(XmlSkill skill, int skillLevel = 1)
	{
		var skillData = GetSkillLevelData(skill, skillLevel);
		if (skillData == null)
		{
			return null;
		}

		return new SkillResourceCosts
		{
			HpCost = skillData.UseHP,
			HpCostStyle = (ResourceCostStyle)skillData.UseHPStyle,
			MpCost = skillData.UseMP,
			MpCostStyle = (ResourceCostStyle)skillData.UseMPStyle,
			SpCost = skillData.UseSP,
			SpCostStyle = (ResourceCostStyle)skillData.UseSPStyle,
			LpCost = skillData.UseLP,
			LpCostStyle = (ResourceCostStyle)skillData.UseLPStyle
		};
	}

	/// <summary>
	/// Gets the skill level data from the skill definition.
	/// </summary>
	private static SkillSubjectList? GetSkillLevelData(XmlSkill skill, int skillLevel)
	{
		if (skill.Subject?.SubjectLists == null || skill.Subject.SubjectLists.Count == 0)
		{
			return null;
		}

		// Find matching level (1-based)
		var match = skill.Subject.SubjectLists.FirstOrDefault(s => s.SubjectLevel == skillLevel);
		if (match != null)
		{
			return match;
		}

		// Fallback to first level if specific level not found
		return skill.Subject.SubjectLists.FirstOrDefault();
	}

	/// <summary>
	/// Calculates the actual resource cost based on style.
	/// </summary>
	private static int CalculateCost(int baseCost, int costStyle, int currentValue, int maxValue)
	{
		return (ResourceCostStyle)costStyle switch
		{
			ResourceCostStyle.Percentage => (int)(maxValue * (baseCost / 100.0)),
			_ => baseCost // Flat
		};
	}
}

/// <summary>
/// Container for skill resource costs.
/// </summary>
public class SkillResourceCosts
{
	/// <summary>HP cost amount.</summary>
	public int HpCost { get; set; }

	/// <summary>HP cost style (flat or percentage).</summary>
	public ResourceCostStyle HpCostStyle { get; set; }

	/// <summary>MP cost amount.</summary>
	public int MpCost { get; set; }

	/// <summary>MP cost style (flat or percentage).</summary>
	public ResourceCostStyle MpCostStyle { get; set; }

	/// <summary>SP cost amount.</summary>
	public int SpCost { get; set; }

	/// <summary>SP cost style (flat or percentage).</summary>
	public ResourceCostStyle SpCostStyle { get; set; }

	/// <summary>LP cost amount.</summary>
	public int LpCost { get; set; }

	/// <summary>LP cost style (flat or percentage).</summary>
	public ResourceCostStyle LpCostStyle { get; set; }

	/// <summary>Returns true if skill has any resource cost.</summary>
	public bool HasAnyCost => HpCost > 0 || MpCost > 0 || SpCost > 0 || LpCost > 0;

	/// <summary>String representation of costs.</summary>
	public override string ToString()
	{
		var costs = new List<string>();
		if (HpCost > 0) costs.Add($"HP:{HpCost}{(HpCostStyle == ResourceCostStyle.Percentage ? "%" : "")}");
		if (MpCost > 0) costs.Add($"MP:{MpCost}{(MpCostStyle == ResourceCostStyle.Percentage ? "%" : "")}");
		if (SpCost > 0) costs.Add($"SP:{SpCost}{(SpCostStyle == ResourceCostStyle.Percentage ? "%" : "")}");
		if (LpCost > 0) costs.Add($"LP:{LpCost}{(LpCostStyle == ResourceCostStyle.Percentage ? "%" : "")}");
		return costs.Count > 0 ? string.Join(", ", costs) : "No cost";
	}
}
