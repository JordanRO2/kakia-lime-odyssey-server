using kakia_lime_odyssey_server.Interfaces;

namespace kakia_lime_odyssey_server.Services.Combat;

/// <summary>
/// Interface for healing-related services.
/// </summary>
public interface IHealingService
{
	/// <summary>
	/// Calculates and applies healing from source to target.
	/// </summary>
	HealResult HealTarget(IEntity source, IEntity target, int skillId, int skillLevel);

	/// <summary>
	/// Calculates and applies self-healing.
	/// </summary>
	HealResult HealSelf(IEntity source, int skillId, int skillLevel);

	/// <summary>
	/// Checks if a heal is a critical heal.
	/// </summary>
	bool RollCriticalHeal(int critRate);

	/// <summary>
	/// Calculates natural HP regeneration amount.
	/// </summary>
	uint CalculateNaturalRegenHP(IEntity entity);

	/// <summary>
	/// Calculates natural MP regeneration amount.
	/// </summary>
	uint CalculateNaturalRegenMP(IEntity entity);
}

/// <summary>
/// Result of a healing calculation.
/// </summary>
public class HealResult
{
	/// <summary>Amount healed</summary>
	public uint HealAmount { get; set; }

	/// <summary>Whether this was a critical heal (bonus healing)</summary>
	public bool IsCritical { get; set; }

	/// <summary>Target's HP after healing</summary>
	public uint NewHP { get; set; }

	/// <summary>Target's max HP</summary>
	public uint MaxHP { get; set; }

	/// <summary>Whether the target was at full health before healing (overheal)</summary>
	public bool WasOverheal { get; set; }

	/// <summary>Packet to send to clients</summary>
	public byte[]? Packet { get; set; }

	/// <summary>HP change notification packet</summary>
	public byte[]? HPChangePacket { get; set; }
}
