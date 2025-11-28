using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet containing equipped items list.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_EQUIP_ITEM_LIST
/// Size: 4 bytes total (header only)
/// Memory Layout (IDA):
/// - 0x00: PACKET_VAR header (4 bytes) - handled by IPacketVar
/// Note: Variable-length array of EQUIP_ITEM (320 bytes each) follows
/// Triggered by: CS_START_GAME
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_EQUIP_ITEM_LIST : IPacketVar
{
	// Variable-length array of equipped items handled separately
}
