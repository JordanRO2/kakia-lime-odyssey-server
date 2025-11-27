/// <summary>
/// Server->Client notification that a buff/debuff was removed from an object.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_REMOVE_DEF
/// Size: 12 bytes (14 with PACKET_FIX header)
/// Triggered by: Buff expiration, dispel, or manual removal
///
/// Memory Layout (from IDA):
/// +0x00 [2 bytes] PACKET_FIX header (implicit)
/// +0x02 [8 bytes] objInstID (object losing the effect)
/// +0x0A [4 bytes] instID (effect instance ID to remove)
/// Total: 14 bytes
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_REMOVE_DEF
{
	/// <summary>Object instance ID that is losing the effect</summary>
	public long objInstID;

	/// <summary>Effect instance ID to remove</summary>
	public uint instID;
}
