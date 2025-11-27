/// <summary>
/// Server packet sent when the exchange completed successfully.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_EXCHANGE_SUCCESS
/// Size: 2 bytes (header only)
/// Ordinal: 2757
/// Sent to both players when the trade has been completed and items transferred.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_EXCHANGE_SUCCESS
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;
}
