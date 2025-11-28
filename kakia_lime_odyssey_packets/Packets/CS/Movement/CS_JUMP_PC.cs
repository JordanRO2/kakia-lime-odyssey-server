using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client sends this packet when the player character jumps.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_JUMP_PC
/// Size: 36 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: FPOS pos (12 bytes)
/// - 0x0E: float deltaLookAtRadian (4 bytes)
/// - 0x12: FPOS dir (12 bytes)
/// - 0x1E: unsigned int tick (4 bytes)
/// - 0x22: bool isSwim (1 byte)
/// - 0x23: char dirType (1 byte)
/// Response: SC_JUMP_PC
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_JUMP_PC : IPacketFixed
{
	public FPOS pos;
	public float deltaLookAtRadian;
	public FPOS dir;
	public uint tick;
	[MarshalAs(UnmanagedType.U1)]
	public bool isSwim;
	public sbyte dirType;
}
