using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server responds to CS_PING with current server tick.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_PONG
/// Size: 6 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: unsigned int tick (4 bytes) - Server tick timestamp
/// Triggered by: CS_PING from client
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_PONG : IPacketFixed
{
	/// <summary>Server tick timestamp (offset 0x02)</summary>
	public uint tick;
}
