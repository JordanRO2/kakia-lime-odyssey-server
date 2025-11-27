/// <summary>
/// Server->Client packet when a teleporter/portal object leaves the player's sight.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_LEAVE_SIGHT_TRANSFER
/// Size: 8 bytes (10 with PACKET_FIX header)
/// Triggered By: Teleporter object out of proximity range
/// Note: This packet is a wrapper around PACKET_SC_LEAVE_ZONEOBJ structure.
/// Fields:
/// - objInstID: Teleporter object instance ID to remove (8 bytes, __int64)
/// </remarks>
using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_LEAVE_SIGHT_TRANSFER : IPacketFixed
{
	/// <summary>Teleporter object instance ID leaving sight</summary>
	public long objInstID;
}
