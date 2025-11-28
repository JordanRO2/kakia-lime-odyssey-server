/// <summary>
/// Client packet sent to unready and make changes to the exchange offer.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_EXCHANGE_AGAIN
/// Size: 2 bytes (header only)
/// Ordinal: 2752
/// Sent when player wants to modify their offer after marking as ready.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_EXCHANGE_AGAIN
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;
}
