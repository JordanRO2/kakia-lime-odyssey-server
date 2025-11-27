using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet for sliding movement.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_SLIDE_PC
/// Size: 47 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: FPOS pos (12 bytes)
/// - 0x0E: float deltaLookAtRadian (4 bytes)
/// - 0x12: FPOS dir (12 bytes)
/// - 0x1E: unsigned int tick (4 bytes)
/// - 0x22: float turningSpeed (4 bytes)
/// - 0x26: unsigned char moveType (1 byte)
/// - 0x27: float deltaBodyRadian (4 bytes)
/// - 0x2B: float velDecRatio (4 bytes)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_SLIDE_PC : IPacketFixed
{
	public FPOS pos;
	public float deltaLookAtRadian;
	public FPOS dir;
	public uint tick;
	public float turningSpeed;
	public byte moveType;
	public float deltaBodyRadian;
	public float velDecRatio;
}
