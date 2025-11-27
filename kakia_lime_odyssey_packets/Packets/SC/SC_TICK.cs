using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server sends periodic tick updates to clients for synchronization.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_TICK
/// Size: 6 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (ushort header) - 2 bytes (handled by IPacketFixed)
/// - 0x02: unsigned int tick - 4 bytes
/// Triggered by: Periodic server tick updates
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_TICK : IPacketFixed
{
	public uint tick;
}
