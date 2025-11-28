/// <summary>
/// Client packet sent to mark player as ready to confirm the exchange.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_EXCHANGE_READY
/// Size: 2 bytes (header only)
/// Ordinal: 2749
/// Sent when player clicks the "Ready" button in the trade window.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_EXCHANGE_READY
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;
}
