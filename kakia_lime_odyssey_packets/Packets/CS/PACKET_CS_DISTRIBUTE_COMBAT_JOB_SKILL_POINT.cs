/// <summary>
/// Client packet to distribute combat job skill points to a specific skill.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_DISTRIBUTE_COMBAT_JOB_SKILL_POINT
/// Size: 8 bytes
/// Ordinal: 2612
/// Player allocates skill points to learn or upgrade a combat job skill.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_DISTRIBUTE_COMBAT_JOB_SKILL_POINT
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Skill type ID to distribute points to</summary>
	public uint skillTypeID;

	/// <summary>Number of points to distribute</summary>
	public ushort point;
}
