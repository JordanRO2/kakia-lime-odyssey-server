/// <summary>
/// Server->Client notification that party member looted an item.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_PARTY_MEMBER_LOOTED_ITEM
/// Size: 16 bytes (18 with PACKET_FIX header)
/// Triggered by: Member loots item
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_PARTY_MEMBER_LOOTED_ITEM
{
	/// <summary>Party member index</summary>
	public uint idx;

	/// <summary>Item type ID</summary>
	public int typeID;

	/// <summary>Item count/quantity</summary>
	public long count;
}
