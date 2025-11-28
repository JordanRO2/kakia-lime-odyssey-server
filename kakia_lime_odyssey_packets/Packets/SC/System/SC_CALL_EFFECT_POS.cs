using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet to play a visual effect at a specific position.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_CALL_EFFECT_POS
/// Size: 18 bytes
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes)
/// - 0x02: POS pos (12 bytes) - World position to spawn effect
/// - 0x0E: int effectID (4 bytes) - Effect ID to play
/// Used to trigger visual effects at a specific world position (AoE effects, ground-targeted skills, etc).
/// Effect is stationary at the specified position.
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_CALL_EFFECT_POS : IPacketFixed
{
	public POS pos;
	public int effectID;
}
