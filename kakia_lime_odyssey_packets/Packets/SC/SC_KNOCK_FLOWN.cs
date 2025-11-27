using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet sent when an entity is knocked up/launched into the air.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_KNOCK_FLOWN
/// Size: 52 bytes (54 with PACKET_FIX header)
/// Triggered by: Skills or attacks that cause launch/knockup effect
/// Used for knockback with vertical component (launches entity into air)
/// Similar to SC_KNOCK_PUSHED but includes verticalSpeed for upward movement
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_KNOCK_FLOWN
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
