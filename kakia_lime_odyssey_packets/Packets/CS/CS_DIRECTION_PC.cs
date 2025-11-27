using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// IDA Verification: PACKET_CS_DIRECTION_PC
/// Size: 18 bytes
/// Structure verified: 2025-11-26
/// Fields:
///   +0x00 PACKET_FIX (2 bytes) - packet header (implicit)
///   +0x02 FPOS dir (12 bytes) - direction vector
///   +0x0E unsigned int tick (4 bytes)
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_DIRECTION_PC
{
	public FPOS dir;
	public uint tick;
}
