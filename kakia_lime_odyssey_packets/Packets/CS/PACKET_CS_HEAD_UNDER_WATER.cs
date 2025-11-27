/// <summary>
/// Client packet notifying server that player's head is underwater.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_HEAD_UNDER_WATER
/// Size: 2 bytes
/// Ordinal: 2594
/// Sent when the player character goes underwater.
/// Server responds with SC_START_HOLDING_BREATH to begin oxygen depletion.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_HEAD_UNDER_WATER
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;
}
