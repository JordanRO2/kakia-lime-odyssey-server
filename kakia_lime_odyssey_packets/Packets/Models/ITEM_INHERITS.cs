using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

/// <summary>
/// Represents a single inherited stat or property on an item.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: ITEM_INHERIT
/// Size: 12 bytes
/// Memory Layout (IDA):
/// - 0x00: unsigned int typeID (4 bytes) - Inherit type identifier
/// - 0x04: int value (4 bytes) - Inherit value/amount
/// - 0x08: unsigned __int8 type (1 byte) - Inherit category type
/// - 0x09-0x0B: padding (3 bytes) - Alignment to 12 bytes
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ITEM_INHERIT
{
	/// <summary>Inherit type identifier (offset 0x00)</summary>
	public uint typeID;

	/// <summary>Inherit value/amount (offset 0x04)</summary>
	public int value;

	/// <summary>Inherit category type (offset 0x08)</summary>
	public byte type;

	/// <summary>Padding byte 1 (offset 0x09)</summary>
	public byte padding1;

	/// <summary>Padding byte 2 (offset 0x0A)</summary>
	public byte padding2;

	/// <summary>Padding byte 3 (offset 0x0B)</summary>
	public byte padding3;
}

/// <summary>
/// Container for item inherit slots (enchantments, bonuses, etc).
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: ITEM_INHERITS
/// Size: 300 bytes (25 x 12 bytes)
/// Memory Layout (IDA):
/// - 0x00: ITEM_INHERIT[25] inherits (300 bytes) - Array of 25 inherit slots
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ITEM_INHERITS
{
	/// <summary>Array of 25 item inherit slots (offset 0x00)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 25)]
	public ITEM_INHERIT[] inherits;
}
