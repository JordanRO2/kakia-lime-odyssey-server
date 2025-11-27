using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet when entity is knocked/pushed back.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_KNOCK_PUSHED
/// Size: 50 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: __int64 instID (8 bytes)
/// - 0x0A: FPOS pos (12 bytes)
/// - 0x16: FPOS dir (12 bytes)
/// - 0x22: float deltaLookAtRadian (4 bytes)
/// - 0x26: unsigned int tick (4 bytes)
/// - 0x2A: float velocity (4 bytes)
/// - 0x2E: float accel (4 bytes)
/// Triggered by: Skills or attacks with knockback effect
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_KNOCK_PUSHED : IPacketFixed
{
	/// <summary>Instance ID of entity being knocked back (offset 0x02)</summary>
	public long instID;

	/// <summary>Current position of the entity (offset 0x0A)</summary>
	public FPOS pos;

	/// <summary>Direction vector of the knockback (offset 0x16)</summary>
	public FPOS dir;

	/// <summary>Change in look-at angle in radians (offset 0x22)</summary>
	public float deltaLookAtRadian;

	/// <summary>Server tick when knockback started (offset 0x26)</summary>
	public uint tick;

	/// <summary>Initial knockback velocity (offset 0x2A)</summary>
	public float velocity;

	/// <summary>Acceleration/deceleration of knockback (offset 0x2E)</summary>
	public float accel;
}
