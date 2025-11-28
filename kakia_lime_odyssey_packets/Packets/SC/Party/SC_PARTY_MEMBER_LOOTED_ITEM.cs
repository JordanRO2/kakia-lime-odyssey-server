using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client notification that party member looted an item.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_PARTY_MEMBER_LOOTED_ITEM
/// Size: 18 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: unsigned int idx (4 bytes)
/// - 0x06: int itemTypeID (4 bytes)
/// - 0x0A: __int64 count (8 bytes)
/// Triggered by: Member loots item
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_PARTY_MEMBER_LOOTED_ITEM : IPacketFixed
{
	/// <summary>Party member index (offset 0x02)</summary>
	public uint idx;

	/// <summary>Item type ID (offset 0x06)</summary>
	public int itemTypeID;

	/// <summary>Item count/quantity (offset 0x0A)</summary>
	public long count;
}
