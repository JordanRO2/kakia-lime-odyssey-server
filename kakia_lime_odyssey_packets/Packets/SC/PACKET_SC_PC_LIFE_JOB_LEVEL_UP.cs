/// <summary>
/// Server packet sent when a player levels up their life job.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_PC_LIFE_JOB_LEVEL_UP
/// Size: 17 bytes
/// Ordinal: 2607
/// Broadcast to nearby players when someone levels up their life job.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_PC_LIFE_JOB_LEVEL_UP
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the player who leveled up</summary>
	public long objInstID;

	/// <summary>New life job level</summary>
	public byte lv;

	/// <summary>Current experience at new level</summary>
	public uint exp;

	/// <summary>Status points gained from level up</summary>
	public ushort statusPoint;
}
