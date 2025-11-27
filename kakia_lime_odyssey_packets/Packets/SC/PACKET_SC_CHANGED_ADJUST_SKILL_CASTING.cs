/// <summary>
/// Server packet sent when skill casting adjustment changes.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_CHANGED_ADJUST_SKILL_CASTING
/// Size: 18 bytes
/// Ordinal: 22311
/// Affects the casting time for skills (separate from casting ratio).
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_CHANGED_ADJUST_SKILL_CASTING
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the object</summary>
	public long objInstID;

	/// <summary>Current skill casting adjustment value</summary>
	public int current;

	/// <summary>Skill casting adjustment change amount (delta)</summary>
	public int update;
}
