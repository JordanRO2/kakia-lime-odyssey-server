/// <summary>
/// Server packet sent when skill applying range adjustment changes.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_CHANGED_ADJUST_SKILL_APPLYING_RANGE
/// Size: 18 bytes
/// Ordinal: 22008
/// Affects the range at which skills can be applied/cast.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_CHANGED_ADJUST_SKILL_APPLYING_RANGE : IPacketFixed
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the object</summary>
	public long objInstID;

	/// <summary>Current skill applying range value</summary>
	public int current;

	/// <summary>Skill applying range change amount (delta)</summary>
	public int update;

	public const int Size = 18;
}
