using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Interface;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet to play a visual effect on an object.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_CALL_EFFECT
/// Size: 14 bytes
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes)
/// - 0x02: __int64 objInstID (8 bytes) - Object instance ID to attach effect to
/// - 0x0A: int effectID (4 bytes) - Effect ID to play
/// Used to trigger visual effects attached to objects (skill effects, buff/debuff visuals, etc).
/// Effect is attached to and follows the object.
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_CALL_EFFECT : IPacketFixed
{
	public long objInstID;
	public int effectID;
}
