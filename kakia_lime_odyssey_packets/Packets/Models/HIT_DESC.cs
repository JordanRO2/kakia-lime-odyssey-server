using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

/// <summary>
/// Hit description structure for weapon hit results
/// Verified against IDA: HIT_DESC (Size: 16 bytes)
/// Date: 2025-11-26
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct HIT_DESC
{
	/// <summary>
	/// Hit result type (0x0)
	/// IDA: unsigned __int8 result
	/// </summary>
	public byte result;

	// 3 bytes of padding here to align to 4-byte boundary

	/// <summary>
	/// Weapon type identifier (0x4)
	/// IDA: int weaponTypeID
	/// </summary>
	public int weaponTypeID;

	/// <summary>
	/// Primary damage amount (0x8)
	/// IDA: unsigned int damage
	/// </summary>
	public uint damage;

	/// <summary>
	/// Bonus damage amount (0xC)
	/// IDA: unsigned int bonusDamage
	/// </summary>
	public uint bonusDamage;
}
