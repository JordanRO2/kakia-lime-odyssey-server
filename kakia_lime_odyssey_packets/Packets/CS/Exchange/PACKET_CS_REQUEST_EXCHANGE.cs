/// <summary>
/// Client packet sent to confirm exchange request after target selection.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_REQUEST_EXCHANGE
/// Size: 2 bytes (header only)
/// Ordinal: 2737
/// Sent after CS_SELECT_TARGET_REQUEST_EXCHANGE to confirm the exchange initiation.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_REQUEST_EXCHANGE
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;
}
