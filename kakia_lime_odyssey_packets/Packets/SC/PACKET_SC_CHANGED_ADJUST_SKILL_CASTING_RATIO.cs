/// <summary>
/// Server packet sent when skill casting ratio adjustment changes.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_CHANGED_ADJUST_SKILL_CASTING_RATIO
/// Size: 18 bytes
/// Ordinal: 19121
/// Affects the casting speed multiplier for skills.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_CHANGED_ADJUST_SKILL_CASTING_RATIO : IPacketFixed
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the object</summary>
	public long objInstID;

	/// <summary>Current skill casting ratio value</summary>
	public int current;

	/// <summary>Skill casting ratio change amount (delta)</summary>
	public int update;

	public const int Size = 18;
}
