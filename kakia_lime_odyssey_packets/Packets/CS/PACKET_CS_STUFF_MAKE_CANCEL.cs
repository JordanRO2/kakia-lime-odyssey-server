/// <summary>
/// Client packet requesting to cancel stuff/material making (gathering).
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_STUFF_MAKE_CANCEL
/// Size: 2 bytes
/// Ordinal: 2643
/// Sent when canceling the gathering/processing action.
/// Server responds with SC_STUFF_MAKE_FINISH.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_STUFF_MAKE_CANCEL
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;
}
