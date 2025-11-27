/// <summary>
/// Client packet notifying server that player's head is above water.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_HEAD_OVER_WATER
/// Size: 2 bytes
/// Ordinal: 2595
/// Sent when the player character surfaces from underwater.
/// Server responds with SC_FINISH_HOLDING_BREATH to stop oxygen depletion.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_HEAD_OVER_WATER
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;
}
