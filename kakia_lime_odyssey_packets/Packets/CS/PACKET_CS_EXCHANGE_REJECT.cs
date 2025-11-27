/// <summary>
/// Client packet sent to reject an incoming exchange request from another player.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_EXCHANGE_REJECT
/// Size: 2 bytes (header only)
/// Ordinal: 2741
/// Sent in response to receiving a trade request to decline it.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_EXCHANGE_REJECT
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;
}
