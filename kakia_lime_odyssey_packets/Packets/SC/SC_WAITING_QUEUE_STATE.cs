using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_WAITING_QUEUE_STATE - Server waiting queue position
/// IDA Structure: PACKET_SC_WAITING_QUEUE_STATE (6 bytes)
/// Verified against IDA Pro: 2025-11-26
///
/// Server sends this to inform the client of their position in the waiting queue.
/// Used when the server is at capacity and players must wait to enter.
///
/// Structure layout (from IDA):
/// 0x00: PACKET_FIX (2 bytes) - base packet header
/// 0x02: int order - position in queue (1-based, 0 means no queue)
/// Total: 6 bytes
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_WAITING_QUEUE_STATE : IPacketFixed
{
    /// <summary>Position in the waiting queue (1 = first in line, 0 = no queue)</summary>
    public int order;
}
