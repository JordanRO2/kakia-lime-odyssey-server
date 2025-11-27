/// <summary>
/// Server packet to update a party member's combat job level.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_PARTY_UPDATE_MEMBER_COMBAT_JOB_LV
/// Size: 10 bytes
/// Ordinal: 2796
/// Notifies party members when a member's combat job level changes.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_PARTY_UPDATE_MEMBER_COMBAT_JOB_LV
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Party member index</summary>
	public uint idx;

	/// <summary>New combat job level</summary>
	public int lv;
}
