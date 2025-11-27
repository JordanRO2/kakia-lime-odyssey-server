using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_INSERT_SLOT_ITEM - Notify client of new item in inventory slot
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_INSERT_SLOT_ITEM
/// Size: 335 bytes
/// Offsets:
///   0x00: PACKET_FIX header (2 bytes)
///   0x02: ITEM_SLOT slot (5 bytes)
///   0x07: int typeID (4 bytes)
///   0x0B: __int64 count (8 bytes)
///   0x13: int durability (4 bytes)
///   0x17: int mdurability (4 bytes)
///   0x1B: int remainExpiryTime (4 bytes)
///   0x1F: int grade (4 bytes)
///   0x23: ITEM_INHERITS inherits (300 bytes)
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_INSERT_SLOT_ITEM
{
	public ushort header;
	public ITEM_SLOT slot;
	public int typeID;
	public long count;
	public int durability;
	public int mdurability;
	public int remainExpiryTime;
	public int grade;
	public ITEM_INHERITS inherits;
}
