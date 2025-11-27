using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

/// <summary>
/// Hit description structure for weapon hit results.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: HIT_DESC
/// Size: 16 bytes
/// Memory Layout (IDA):
/// - 0x00: unsigned __int8 result (1 byte) - Hit result type
/// - 0x01-0x03: padding (3 bytes) - Alignment to 4-byte boundary
/// - 0x04: int weaponTypeID (4 bytes) - Weapon type identifier
/// - 0x08: unsigned int damage (4 bytes) - Primary damage amount
/// - 0x0C: unsigned int bonusDamage (4 bytes) - Bonus damage amount
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct HIT_DESC
{
	/// <summary>Hit result type (offset 0x00)</summary>
	public byte result;

	/// <summary>Padding byte 1 for alignment (offset 0x01)</summary>
	public byte padding1;

	/// <summary>Padding byte 2 for alignment (offset 0x02)</summary>
	public byte padding2;

	/// <summary>Padding byte 3 for alignment (offset 0x03)</summary>
	public byte padding3;

	/// <summary>Weapon type identifier (offset 0x04)</summary>
	public int weaponTypeID;

	/// <summary>Primary damage amount (offset 0x08)</summary>
	public uint damage;

	/// <summary>Bonus damage amount (offset 0x0C)</summary>
	public uint bonusDamage;
}
