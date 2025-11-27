using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Interface;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet to notify about a combat job selection/choice.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_CHOICE_COMBAT_JOB
/// Size: 12 bytes
/// Memory Layout (IDA):
/// - 0x00: PACKET_VAR header (4 bytes) - Variable-length packet header
/// - 0x04: __int64 objInstID (8 bytes) - Object instance ID that selected a combat job
/// Used to broadcast when a player selects or changes their combat job class.
/// Note: Uses PACKET_VAR header instead of PACKET_FIX (4 bytes vs 2 bytes).
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_CHOICE_COMBAT_JOB : IPacketFixed
{
	public long objInstID;
}
