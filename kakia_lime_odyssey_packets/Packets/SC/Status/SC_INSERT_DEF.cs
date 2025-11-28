using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet to add a buff/debuff effect to the target.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_INSERT_DEF
/// Size: 26 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: DEF def (16 bytes)
/// - 0x12: __int64 objInstID (8 bytes)
/// Triggered by: Buff/debuff application from skill or item
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_INSERT_DEF : IPacketFixed
{
	/// <summary>Buff/debuff effect definition (offset 0x02)</summary>
	public DEF def;

	/// <summary>Target object instance ID (offset 0x12)</summary>
	public long objInstID;
}
