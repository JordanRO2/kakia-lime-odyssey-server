using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Interface;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet to play a sound effect on an object.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_CALL_SOUND
/// Size: 14 bytes
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes)
/// - 0x02: __int64 objInstID (8 bytes) - Object instance ID to play sound from
/// - 0x0A: int soundID (4 bytes) - Sound ID to play
/// Used to trigger sound effects originating from objects (skill sounds, combat sounds, etc).
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_CALL_SOUND : IPacketFixed
{
	public long objInstID;
	public int soundID;
}
