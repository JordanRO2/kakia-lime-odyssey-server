using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet confirming client has re-entered the lobby.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_REENTER_LOBBY
/// Size: 2 bytes total (header only)
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_REENTER_LOBBY : IPacketFixed
{
	// Empty packet - header only
}
