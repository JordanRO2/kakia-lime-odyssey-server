using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client: Slide movement packet
/// IDA Verification: VERIFIED 2025-11-26
/// Structure Name: PACKET_SC_SLIDE_PC
/// Total Size: 67 bytes (2 header + 65 data)
///
/// Field Layout:
/// Offset | Size | Type              | Name
/// -------|------|-------------------|------------------
/// 0x0000 | 2    | PACKET_FIX        | [header]
/// 0x0002 | 8    | __int64           | objInstID
/// 0x000A | 12   | FPOS              | pos
/// 0x0016 | 12   | FPOS              | dir
/// 0x0022 | 4    | float             | deltaLookAtRadian
/// 0x0026 | 4    | unsigned int      | tick
/// 0x002A | 4    | float             | velocity
/// 0x002E | 4    | float             | accel
/// 0x0032 | 4    | float             | turningSpeed
/// 0x0036 | 4    | float             | velocityRatio
/// 0x003A | 1    | unsigned __int8   | moveType
/// 0x003B | 4    | float             | deltaBodyRadian
/// 0x003F | 4    | float             | velDecRatio
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_SLIDE_PC : IPacketFixed
{
	public ushort header;
	public long objInstID;
	public FPOS pos;
	public FPOS dir;
	public float deltaLookAtRadian;
	public uint tick;
	public float velocity;
	public float accel;
	public float turningSpeed;
	public float velocityRatio;
	public byte moveType;
	public float deltaBodyRadian;
	public float velDecRatio;
}
