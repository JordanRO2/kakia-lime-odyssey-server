using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client notification that a buff/debuff was removed from an object.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_REMOVE_DEF
/// Size: 14 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: __int64 objInstID (8 bytes)
/// - 0x0A: unsigned int instID (4 bytes)
/// Triggered by: Buff expiration, dispel, or manual removal
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_REMOVE_DEF : IPacketFixed
{
	/// <summary>Object instance ID that is losing the effect (offset 0x02)</summary>
	public long objInstID;

	/// <summary>Effect instance ID to remove (offset 0x0A)</summary>
	public uint instID;
}
