using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Interface;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet to set an object's race relation state.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_DO_RELATION
/// Size: 14 bytes
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes)
/// - 0x02: __int64 objInstID (8 bytes) - Object instance ID
/// - 0x0A: int raceRelationState (4 bytes) - Relation state (hostile, friendly, neutral, etc)
/// Used to update how races/factions relate to each other for visual/combat purposes.
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_DO_RELATION : IPacketFixed
{
	public long objInstID;
	public int raceRelationState;
}
