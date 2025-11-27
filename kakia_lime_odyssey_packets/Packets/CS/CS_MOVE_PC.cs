using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client sends this packet when the player character starts moving.
/// Contains position, direction, and movement parameters for server-side validation and replication.
/// </summary>
/// <remarks>
/// IDA Verified: Yes
/// IDA Struct: PACKET_CS_MOVE_PC
/// Size: 37 bytes (39 bytes with PACKET_FIX header)
/// IDA Address: Structure ordinal 2470
/// Response: SC_MOVE
///
/// Field Breakdown:
/// - pos (FPOS, 12 bytes): Current position (x, y, z)
/// - deltaLookAtRadian (float, 4 bytes): Change in look-at angle in radians
/// - dir (FPOS, 12 bytes): Direction vector (x, y, z)
/// - tick (uint, 4 bytes): Client timestamp for movement synchronization
/// - turningSpeed (float, 4 bytes): Rate of turning in radians per second
/// - moveType (byte, 1 byte): Type of movement (walk, run, etc.)
///
/// Verified against IDA: 2025-11-26
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_MOVE_PC
{
	public FPOS pos;
	public float deltaLookAtRadian;
	public FPOS dir;
	public uint tick;
	public float turningSpeed;
	public byte moveType;
}
