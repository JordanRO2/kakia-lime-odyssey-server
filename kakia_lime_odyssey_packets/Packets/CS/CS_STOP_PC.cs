using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client requests to stop player character movement.
/// Sent when player stops moving, contains final position and facing direction.
/// </summary>
/// <remarks>
/// IDA Verified: Yes
/// IDA Struct: PACKET_CS_STOP_PC
/// Size: 29 bytes (31 with PACKET_FIX header)
/// Response: SC_STOP
/// Verification Date: 2025-11-26
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_STOP_PC
{
	public FPOS pos;        // Final position (x, y, z)
	public FPOS dir;        // Facing direction (x, y, z)
	public uint tick;       // Client tick/timestamp
	public byte stopType;   // Type of stop (normal, forced, etc.)
}
