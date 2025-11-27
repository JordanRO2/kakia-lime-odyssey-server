using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet sent when a knockback/launch effect ends.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_RELEASE_KNOCK
/// Size: 26 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: __int64 instID (8 bytes)
/// - 0x0A: POS pos (12 bytes)
/// - 0x16: unsigned int tick (4 bytes)
/// Triggered by: End of knockback/launch effect
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_RELEASE_KNOCK : IPacketFixed
{
	/// <summary>Instance ID of entity being released from knockback (offset 0x02)</summary>
	public long instID;

	/// <summary>Final position where entity stopped (offset 0x0A)</summary>
	public POS pos;

	/// <summary>Server tick when knockback ended (offset 0x16)</summary>
	public uint tick;
}
