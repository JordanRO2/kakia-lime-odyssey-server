/// <summary>
/// Server packet sent when an entity's velocity ratio changes (movement speed multiplier).
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_CHANGED_VELOCITY_RATIO
/// Size: 6 bytes
/// Ordinal: 2618
/// The ratio is a multiplier applied to base velocities (e.g., buffs/debuffs affecting speed).
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_CHANGED_VELOCITY_RATIO
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>New velocity ratio multiplier</summary>
	public float ratio;
}
