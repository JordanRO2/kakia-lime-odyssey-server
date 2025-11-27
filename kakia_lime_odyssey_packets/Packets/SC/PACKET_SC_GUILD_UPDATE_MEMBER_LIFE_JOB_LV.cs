/// <summary>
/// Server packet sent to guild members when a member's life job level changes.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_GUILD_UPDATE_MEMBER_LIFE_JOB_LV
/// Size: 10 bytes
/// Ordinal: 2832
/// Updates guild UI with member's life job level.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_GUILD_UPDATE_MEMBER_LIFE_JOB_LV
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Guild member index</summary>
	public uint idx;

	/// <summary>New life job level</summary>
	public int lv;
}
