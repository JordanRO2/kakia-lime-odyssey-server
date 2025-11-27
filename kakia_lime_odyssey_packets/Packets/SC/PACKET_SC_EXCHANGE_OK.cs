/// <summary>
/// Server packet sent to notify that the other player has given final confirmation.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_EXCHANGE_OK
/// Size: 2 bytes (header only)
/// Ordinal: 2755
/// Sent when the other player has clicked the final OK button.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_EXCHANGE_OK
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;
}
