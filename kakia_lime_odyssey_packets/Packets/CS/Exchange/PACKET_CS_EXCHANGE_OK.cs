/// <summary>
/// Client packet sent to finalize and confirm the exchange when both players are ready.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_EXCHANGE_OK
/// Size: 2 bytes (header only)
/// Ordinal: 2754
/// Final confirmation packet sent when both players have marked ready.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_EXCHANGE_OK
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;
}
