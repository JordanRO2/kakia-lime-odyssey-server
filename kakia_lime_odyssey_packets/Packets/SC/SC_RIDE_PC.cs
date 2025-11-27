/// <summary>
/// Server->Client packet sent when a player or NPC mounts a pet/mount.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_RIDE_PC
/// Size: 14 bytes (2-byte header + 12 bytes data)
/// Triggered by: mount action
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_RIDE_PC
{
	/// <summary>Object instance ID of the character mounting</summary>
	public long objInstID;

	/// <summary>Item type ID of the mount/pet being ridden</summary>
	public int rideItemTypeID;
}
