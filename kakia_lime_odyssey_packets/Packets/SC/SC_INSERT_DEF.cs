/// <summary>
/// Server->Client notification that a buff/debuff was applied to an object.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_INSERT_DEF
/// Size: 24 bytes (26 with PACKET_FIX header)
/// Triggered by: Buff/debuff application from skill or item
///
/// Memory Layout (from IDA):
/// +0x00 [2 bytes]  PACKET_FIX header (implicit)
/// +0x02 [16 bytes] DEF structure (buff/debuff data)
/// +0x12 [8 bytes]  objInstID (target object instance ID)
/// Total: 26 bytes
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_INSERT_DEF
{
	/// <summary>Buff/debuff effect definition</summary>
	public DEF def;

	/// <summary>Target object instance ID that received the effect</summary>
	public long objInstID;
}
