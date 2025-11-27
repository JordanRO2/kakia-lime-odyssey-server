namespace kakia_lime_odyssey_packets.Packets.Enums;

/// <summary>
/// Hit result type enum for combat hit results.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Type: AttackInfo::HIT_FAIL_TYPE
/// Used in: HIT_DESC, MakeDamageNumberInfo, AttackInfo structures
/// </remarks>
public enum HIT_FAIL_TYPE : byte
{
	/// <summary>No hit result (default/invalid)</summary>
	HIT_FAIL_NONE = 0x0,

	/// <summary>Normal hit - damage dealt</summary>
	HIT_FAIL_HIT = 0x1,

	/// <summary>Critical hit - bonus damage dealt</summary>
	HIT_FAIL_CRITICAL_HIT = 0x2,

	/// <summary>Miss - attack failed to connect</summary>
	HIT_FAIL_MISS = 0x3,

	/// <summary>Avoid/Dodge - target evaded the attack</summary>
	HIT_FAIL_AVOID = 0x4,

	/// <summary>Shield block - damage blocked by shield</summary>
	HIT_FAIL_SHIELD = 0x5,

	/// <summary>Guard/Parry - damage reduced by parrying</summary>
	HIT_FAIL_GUARD = 0x6
}
