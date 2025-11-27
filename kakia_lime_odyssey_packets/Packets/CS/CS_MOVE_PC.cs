using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client sends this packet when the player character starts moving.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_MOVE_PC
/// Size: 39 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: FPOS pos (12 bytes)
/// - 0x0E: float deltaLookAtRadian (4 bytes)
/// - 0x12: FPOS dir (12 bytes)
/// - 0x1E: unsigned int tick (4 bytes)
/// - 0x22: float turningSpeed (4 bytes)
/// - 0x26: unsigned char moveType (1 byte)
/// Response: SC_MOVE
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_MOVE_PC : IPacketFixed
{
	public FPOS pos;
	public float deltaLookAtRadian;
	public FPOS dir;
	public uint tick;
	public float turningSpeed;
	public byte moveType;
}
