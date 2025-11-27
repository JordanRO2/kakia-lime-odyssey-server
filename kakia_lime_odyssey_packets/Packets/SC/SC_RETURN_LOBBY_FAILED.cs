using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server notifies client that returning to lobby has failed.
/// This packet has no payload beyond the header.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_RETURN_LOBBY_FAILED
/// Size: 2 bytes total (header only)
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (ushort header) - 2 bytes (handled by IPacketFixed)
/// Triggered by: Failed CS_RETURN_LOBBY attempt
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_RETURN_LOBBY_FAILED : IPacketFixed
{
}
