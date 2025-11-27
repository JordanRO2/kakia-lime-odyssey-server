/// <summary>
/// Server packet sent to both players to start the exchange window.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_START_EXCHANGE
/// Size: 10 bytes
/// Ordinal: 2742
/// Fields:
/// - target (offset 0x02, 8 bytes): Instance ID of the other player in the trade
/// Sent after both players have agreed to start the exchange.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_START_EXCHANGE
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the other player in the exchange</summary>
	public long target;
}
