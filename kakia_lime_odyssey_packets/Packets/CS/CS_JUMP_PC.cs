using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client sends this packet when the player character jumps.
/// Contains position, direction, and jump parameters for server-side validation and replication.
/// </summary>
/// <remarks>
/// IDA Verified: Yes
/// IDA Struct: PACKET_CS_JUMP_PC
/// Size: 34 bytes (36 bytes with PACKET_FIX header)
/// IDA Address: Structure ordinal 2475
/// Response: SC_JUMP_PC
///
/// Field Breakdown:
/// - pos (FPOS, 12 bytes): Current position at jump start (x, y, z)
/// - deltaLookAtRadian (float, 4 bytes): Change in look-at angle in radians
/// - dir (FPOS, 12 bytes): Direction vector (x, y, z)
/// - tick (uint, 4 bytes): Client timestamp for jump synchronization
/// - isSwim (bool, 1 byte): Whether character is swimming when jumping
/// - dirType (sbyte/char, 1 byte): Direction type flag
///
/// Verified against IDA: 2025-11-26
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_JUMP_PC
{
	public FPOS pos;
	public float deltaLookAtRadian;
	public FPOS dir;
	public uint tick;
	[MarshalAs(UnmanagedType.U1)]
	public bool isSwim;
	public sbyte dirType;
}
