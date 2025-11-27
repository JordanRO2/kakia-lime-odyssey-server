using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_BOARD_FINISH - Quest board closed
/// IDA Structure: PACKET_SC_BOARD_FINISH (2 bytes)
/// Verified against IDA Pro: 2025-11-26
///
/// Server sends this to close the quest board UI on the client.
///
/// Structure layout:
/// 0x00: PACKET_FIX (2 bytes) - base packet header
/// Total: 2 bytes
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_BOARD_FINISH : IPacketFixed
{
    // Header only packet - no additional fields
}
