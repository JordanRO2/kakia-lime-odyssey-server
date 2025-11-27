using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure Name: PACKET_SC_EQUIP_ITEM_INFO
/// Size: 319 bytes (0x13F)
/// Server sends detailed information about an equipped item in response to CS_EQUIP_ITEM_INFO
/// </summary>
/// <remarks>
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX (base header) - 2 bytes [handled by IPacketFixed]
/// - 0x02: unsigned __int8 equipSlot - 1 byte (equipment slot index)
/// - 0x03: int typeID - 4 bytes (item template/type ID)
/// - 0x07: int durability - 4 bytes (current durability)
/// - 0x0B: int mdurability - 4 bytes (maximum durability)
/// - 0x0F: int grade - 4 bytes (item grade/quality)
/// - 0x13: ITEM_INHERITS inherits - 300 bytes (inherited stats/properties)
/// Total: 319 bytes
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_EQUIP_ITEM_INFO : IPacketFixed
{
	public byte equipSlot;         // Offset: 0x02 - Equipment slot index
	public int typeID;             // Offset: 0x03 - Item type/template ID
	public int durability;         // Offset: 0x07 - Current durability
	public int mdurability;        // Offset: 0x0B - Maximum durability
	public int grade;              // Offset: 0x0F - Item grade/quality
	public ITEM_INHERITS inherits; // Offset: 0x13 - Inherited stats (300 bytes)
}
