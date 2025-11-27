using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client: Direction change notification
/// IDA Verification: VERIFIED 2025-11-26
/// Structure Name: PACKET_SC_DIRECTION
/// Total Size: 26 bytes (2 header + 24 data)
///
/// Field Layout:
/// Offset | Size | Type              | Name
/// -------|------|-------------------|------------------
/// 0x0000 | 2    | PACKET_FIX        | [header]
/// 0x0002 | 8    | __int64           | objInstID
/// 0x000A | 12   | FPOS              | dir
/// 0x0016 | 4    | unsigned int      | tick
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_DIRECTION : IPacketFixed
{
	public ushort header;
	public long objInstID;
	public FPOS dir;
	public uint tick;
}
