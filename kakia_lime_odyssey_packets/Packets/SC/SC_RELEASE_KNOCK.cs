using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet sent when a knockback/launch effect ends.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_RELEASE_KNOCK
/// Size: 24 bytes (26 with PACKET_FIX header)
/// Triggered by: End of knockback/launch effect (landed or effect duration expired)
/// Sent after SC_KNOCK_PUSHED or SC_KNOCK_FLOWN to restore normal control to entity
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_RELEASE_KNOCK
{
	/// <summary>Instance ID of the entity being released from knockback</summary>
	public long instID;

	/// <summary>Final position where entity stopped (integer coordinates)</summary>
	public POS pos;

	/// <summary>Server tick when knockback ended</summary>
	public uint tick;
}
