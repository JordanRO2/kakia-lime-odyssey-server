/// <summary>
/// Client packet for system notices (variable length).
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_NOTICE
/// Size: 4 bytes (header only, additional data follows)
/// Ordinal: 2428
/// Variable-length packet for system notice messages.
/// Server responds with SC_NOTICE.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_NOTICE
{
	/// <summary>Packet header with size</summary>
	public PACKET_VAR header;
}
