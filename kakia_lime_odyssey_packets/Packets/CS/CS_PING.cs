using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// IDA Verification: PACKET_CS_PING
/// Size: 6 bytes
/// Structure verified: 2025-11-26
/// Fields:
///   +0x00 PACKET_FIX (2 bytes) - packet header (implicit)
///   +0x02 unsigned int tick (4 bytes)
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_PING
{
	public uint tick;
}
