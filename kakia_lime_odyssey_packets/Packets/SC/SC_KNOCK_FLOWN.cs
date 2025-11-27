using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet when entity is knocked up/launched into the air.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_KNOCK_FLOWN
/// Size: 54 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: __int64 instID (8 bytes)
/// - 0x0A: FPOS pos (12 bytes)
/// - 0x16: FPOS dir (12 bytes)
/// - 0x22: float deltaLookAtRadian (4 bytes)
/// - 0x26: unsigned int tick (4 bytes)
/// - 0x2A: float velocity (4 bytes)
/// - 0x2E: float accel (4 bytes)
/// - 0x32: float verticalSpeed (4 bytes)
/// Triggered by: Skills or attacks with knockup effect
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_KNOCK_FLOWN : IPacketFixed
{
	/// <summary>Instance ID of the entity being launched</summary>
	public long instID;

	/// <summary>Current position of the entity</summary>
	public FPOS pos;

	/// <summary>Direction vector of the knockback</summary>
	public FPOS dir;

	/// <summary>Change in look-at angle (radians)</summary>
	public float deltaLookAtRadian;

	/// <summary>Server tick when launch started</summary>
	public uint tick;

	/// <summary>Initial horizontal velocity</summary>
	public float velocity;

	/// <summary>Horizontal acceleration/deceleration (usually negative for friction)</summary>
	public float accel;

	/// <summary>Initial vertical speed (upward velocity)</summary>
	public float verticalSpeed;
}
