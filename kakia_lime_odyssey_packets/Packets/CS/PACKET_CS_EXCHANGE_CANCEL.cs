/// <summary>
/// Client packet sent to cancel the ongoing exchange.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_EXCHANGE_CANCEL
/// Size: 2 bytes (header only)
/// Ordinal: 2756
/// Sent when player clicks cancel button or closes the trade window.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_EXCHANGE_CANCEL
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;
}
