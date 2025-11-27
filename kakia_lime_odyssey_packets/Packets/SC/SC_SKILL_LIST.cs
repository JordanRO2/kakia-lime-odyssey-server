using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet containing the player's skill list.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_SKILL_LIST
/// Size: 4 bytes total (header only)
/// Memory Layout (IDA):
/// - 0x00: PACKET_VAR header (4 bytes) - handled by IPacketVar
/// Note: Variable-length array of SKILL (12 bytes each) follows
/// SKILL struct: typeID (4), level (2), padding (2), remainCoolTime (4)
/// Triggered by: CS_START_GAME, skill changes
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_SKILL_LIST : IPacketVar
{
	// Note: Variable-length array of SKILL follows, handled separately
}
