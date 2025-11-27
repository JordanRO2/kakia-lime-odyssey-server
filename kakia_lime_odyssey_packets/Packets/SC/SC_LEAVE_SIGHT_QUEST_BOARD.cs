using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet when a quest board leaves the player's view range.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_LEAVE_SIGHT_QUEST_BOARD
/// Size: 10 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_SC_LEAVE_ZONEOBJ (10 bytes) - base zone object leave data
///   - 0x00: PACKET_FIX header (2 bytes)
///   - 0x02: __int64 objInstID (8 bytes)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_LEAVE_SIGHT_QUEST_BOARD : IPacketFixed
{
	/// <summary>Base zone object leave data</summary>
	public SC_LEAVE_ZONEOBJ baseData;
}
