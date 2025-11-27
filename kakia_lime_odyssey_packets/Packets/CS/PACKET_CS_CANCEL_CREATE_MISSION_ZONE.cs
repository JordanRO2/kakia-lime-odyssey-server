/// <summary>
/// Client packet requesting to cancel mission zone creation.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_CANCEL_CREATE_MISSION_ZONE
/// Size: 2 bytes
/// Ordinal: 2454
/// Simple request packet to cancel the creation of a mission zone instance.
/// Server responds with SC_CANCELED_CREATE_MISSION_ZONE.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_CANCEL_CREATE_MISSION_ZONE
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;
}
