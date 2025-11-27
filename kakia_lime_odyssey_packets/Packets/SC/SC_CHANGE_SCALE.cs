using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Interface;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet to change an object's visual scale/size.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_CHANGE_SCALE
/// Size: 15 bytes
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes)
/// - 0x02: __int64 objInstID (8 bytes) - Object instance ID
/// - 0x0A: float scale (4 bytes) - Scale multiplier (1.0 = normal size)
/// - 0x0E: bool smooth (1 byte) - Whether to smoothly transition to new scale
/// Used for size-changing buffs, skills, or visual effects.
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_CHANGE_SCALE : IPacketFixed
{
	public long objInstID;
	public float scale;
	[MarshalAs(UnmanagedType.U1)]
	public bool smooth;
}
