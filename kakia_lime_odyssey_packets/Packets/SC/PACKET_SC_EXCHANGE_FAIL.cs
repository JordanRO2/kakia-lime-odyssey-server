/// <summary>
/// Server packet sent when the exchange failed or was cancelled.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_EXCHANGE_FAIL
/// Size: 2 bytes (header only)
/// Ordinal: 2758
/// Sent when the trade is cancelled, fails validation, or encounters an error.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_EXCHANGE_FAIL
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;
}
