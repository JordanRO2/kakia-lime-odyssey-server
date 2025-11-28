using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client: Jump/movement packet for player character
/// IDA Verification: VERIFIED 2025-11-26
/// Structure Name: PACKET_SC_JUMP_PC
/// Total Size: 55 bytes (2 header + 53 data)
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
/// 0x0032 | 4    | float             | ratio
/// 0x0036 | 1    | bool              | isSwim
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_JUMP_PC : IPacketFixed
{
	public long objInstID;
	public FPOS pos;
	public FPOS dir;
	public float deltaLookAtRadian;
	public uint tick;
	public float velocity;
	public float accel;
	public float ratio;
	[MarshalAs(UnmanagedType.U1)]
	public bool isSwim;
}
