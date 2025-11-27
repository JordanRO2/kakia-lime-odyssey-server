/// <summary>
/// Server packet sent when an item's casting is delayed or has remaining time updated.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_DELAYED_CASTING_ITEM
/// Size: 14 bytes
/// Ordinal: 2644
/// Updates the client on remaining cast time for an item being cast.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_DELAYED_CASTING_ITEM
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the caster</summary>
	public long instID;

	/// <summary>Remaining casting time in milliseconds</summary>
	public uint remainTime;
}
