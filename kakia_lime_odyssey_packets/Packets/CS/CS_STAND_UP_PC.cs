using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// CS_STAND_UP_PC - Client requests player character to stand up
/// IDA Verification: PACKET_CS_STAND_UP_PC
/// Size: 6 bytes (2 byte header + 4 byte tick)
/// Verified: 2025-11-26
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_STAND_UP_PC
{
	public uint tick;
}
