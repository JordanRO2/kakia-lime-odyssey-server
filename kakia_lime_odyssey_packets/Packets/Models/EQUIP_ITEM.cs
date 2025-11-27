using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: EQUIP_ITEM @ 320 bytes
/// Represents a single equipped item with its properties and inherited stats.
/// </summary>
/// <remarks>
/// Memory Layout (IDA):
/// - 0x00: int itemTypeID - 4 bytes (item template ID)
/// - 0x04: unsigned __int8 equipSlot - 1 byte (equipment slot number)
/// - 0x05: unsigned __int8 wiredSlot - 1 byte (wired slot number)
/// - 0x06: [2 bytes padding for alignment]
/// - 0x08: int durability - 4 bytes (current durability)
/// - 0x0C: int mdurability - 4 bytes (maximum durability)
/// - 0x10: int grade - 4 bytes (item grade/quality)
/// - 0x14: ITEM_INHERITS inherits - 300 bytes (inherited stats array)
/// Total: 320 bytes
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 320)]
public struct EQUIP_ITEM
{
	public int itemTypeID;             // Offset: 0x00 - Item type/template ID
	public byte equipSlot;             // Offset: 0x04 - Equipment slot
	public byte wiredSlot;             // Offset: 0x05 - Wired slot
	private ushort _padding;           // Offset: 0x06 - Padding for alignment (2 bytes)
	public int durability;             // Offset: 0x08 - Current durability
	public int mdurability;            // Offset: 0x0C - Maximum durability
	public int grade;                  // Offset: 0x10 - Item grade/quality
	public ITEM_INHERITS inherits;     // Offset: 0x14 - Inherited stats (300 bytes)
}
