using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet sent when an entity is knocked back (pushed).
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_KNOCK_PUSHED
/// Size: 48 bytes (50 with PACKET_FIX header)
/// Triggered by: Skills or attacks that cause knockback effect
/// Used for horizontal knockback without vertical component (no launch into air)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_KNOCK_PUSHED
{
	/// <summary>Instance ID of the entity being knocked back</summary>
	public long instID;

	/// <summary>Current position of the entity</summary>
	public FPOS pos;

	/// <summary>Direction vector of the knockback</summary>
	public FPOS dir;

	/// <summary>Change in look-at angle (radians)</summary>
	public float deltaLookAtRadian;

	/// <summary>Server tick when knockback started</summary>
	public uint tick;

	/// <summary>Initial knockback velocity</summary>
	public float velocity;

	/// <summary>Acceleration/deceleration of knockback (usually negative for friction)</summary>
	public float accel;
}
