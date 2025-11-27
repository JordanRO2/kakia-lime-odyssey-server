/// <summary>
/// Client packet requesting to start stuff/material making (gathering).
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_STUFF_MAKE_START
/// Size: 2 bytes
/// Ordinal: 2640
/// Sent when starting the gathering/processing action.
/// Server responds with SC_STUFF_MAKE_START_CASTING.
/// Requires database update to inventory when complete.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_STUFF_MAKE_START
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;
}
