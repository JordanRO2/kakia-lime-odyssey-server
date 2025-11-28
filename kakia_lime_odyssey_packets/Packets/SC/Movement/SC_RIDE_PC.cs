using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet sent when a player mounts a pet/mount.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_RIDE_PC
/// Size: 14 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: __int64 objInstID (8 bytes)
/// - 0x0A: int rideItemTypeID (4 bytes)
/// Triggered by: mount action
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_RIDE_PC : IPacketFixed
{
	/// <summary>Object instance ID of the character mounting (offset 0x02)</summary>
	public long objInstID;

	/// <summary>Item type ID of the mount/pet being ridden (offset 0x0A)</summary>
	public int rideItemTypeID;
}
