using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// IDA Verification: PACKET_CS_FELL_PC
/// Size: 6 bytes
/// Structure verified: 2025-11-26
/// Fields:
///   +0x00 PACKET_FIX (2 bytes) - packet header (implicit)
///   +0x02 float velocity (4 bytes) - falling velocity
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_FELL_PC
{
	public float velocity;
}
