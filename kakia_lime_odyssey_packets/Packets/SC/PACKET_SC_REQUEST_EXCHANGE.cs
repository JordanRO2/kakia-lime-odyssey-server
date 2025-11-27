/// <summary>
/// Server packet sent to confirm exchange request has been sent to target player.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_REQUEST_EXCHANGE
/// Size: 2 bytes (header only)
/// Ordinal: 2738
/// Sent to the requesting player after CS_REQUEST_EXCHANGE.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_REQUEST_EXCHANGE
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;
}
