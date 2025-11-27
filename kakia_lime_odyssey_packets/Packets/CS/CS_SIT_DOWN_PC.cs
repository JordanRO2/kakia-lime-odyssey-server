using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// CS_SIT_DOWN_PC - Client requests player character to sit down
/// IDA Verification: PACKET_CS_SIT_DOWN_PC
/// Size: 6 bytes (2 byte header + 4 byte tick)
/// Verified: 2025-11-26
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_SIT_DOWN_PC
{
	public uint tick;
}
