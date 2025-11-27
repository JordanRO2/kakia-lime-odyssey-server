using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client requests to stop player character movement.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_STOP_PC
/// Size: 31 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: FPOS pos (12 bytes)
/// - 0x0E: FPOS dir (12 bytes)
/// - 0x1A: unsigned int tick (4 bytes)
/// - 0x1E: unsigned char stopType (1 byte)
/// Response: SC_STOP
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_STOP_PC : IPacketFixed
{
	public FPOS pos;        // Final position (x, y, z)
	public FPOS dir;        // Facing direction (x, y, z)
	public uint tick;       // Client tick/timestamp
	public byte stopType;   // Type of stop (normal, forced, etc.)
}
