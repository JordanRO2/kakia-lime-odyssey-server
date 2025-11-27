/// <summary>
/// Server packet sent to notify that a player has unreadied to make changes.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_EXCHANGE_AGAIN
/// Size: 2 bytes (header only)
/// Ordinal: 2753
/// Sent when either player wants to modify their offer after marking ready.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_EXCHANGE_AGAIN
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;
}
