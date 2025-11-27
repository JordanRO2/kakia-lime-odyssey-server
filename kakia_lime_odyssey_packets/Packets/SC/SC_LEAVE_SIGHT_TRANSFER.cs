using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet when a teleporter/portal object leaves the player's sight.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_LEAVE_SIGHT_TRANSFER
/// Size: 10 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_SC_LEAVE_ZONEOBJ (10 bytes) - contains PACKET_FIX header + objInstID
/// Note: This packet inherits from PACKET_SC_LEAVE_ZONEOBJ structure.
/// Triggered By: Teleporter object out of proximity range
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_LEAVE_SIGHT_TRANSFER : IPacketFixed
{
	/// <summary>Teleporter object instance ID leaving sight (offset 0x02)</summary>
	public long objInstID;
}
