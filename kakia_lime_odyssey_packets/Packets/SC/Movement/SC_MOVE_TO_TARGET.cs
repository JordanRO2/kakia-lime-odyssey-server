using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client: Move to target location packet
/// IDA Verification: VERIFIED 2025-11-26
/// Structure Name: PACKET_SC_MOVE_TO_TARGET
/// Total Size: 47 bytes (2 header + 45 data)
///
/// Field Layout:
/// Offset | Size | Type              | Name
/// -------|------|-------------------|------------------
/// 0x0000 | 2    | PACKET_FIX        | [header]
/// 0x0002 | 8    | __int64           | objInstID
/// 0x000A | 12   | FPOS              | startPos
/// 0x0016 | 12   | FPOS              | targetPos
/// 0x0022 | 4    | unsigned int      | tick
/// 0x0026 | 4    | float             | velocity
/// 0x002A | 4    | int               | aniId
/// 0x002E | 1    | unsigned __int8   | moveType
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_MOVE_TO_TARGET : IPacketFixed
{
	public ushort header;
	public long objInstID;
	public FPOS startPos;
	public FPOS targetPos;
	public uint tick;
	public float velocity;
	public int aniId;
	public byte moveType;
}
