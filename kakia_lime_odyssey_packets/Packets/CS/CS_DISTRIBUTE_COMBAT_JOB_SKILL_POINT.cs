/// <summary>
/// Client->Server packet to distribute combat job skill points.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_DISTRIBUTE_COMBAT_JOB_SKILL_POINT
/// Size: 6 bytes (8 with PACKET_FIX header)
/// Response: SC_SKILL_LV_UP
/// Note: Player uses skill points gained from combat job level ups to level up skills
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_DISTRIBUTE_COMBAT_JOB_SKILL_POINT
{
	/// <summary>Skill type ID to level up (from SkillInfo.xml)</summary>
	public uint skillTypeID;

	/// <summary>Number of skill points to invest in this skill</summary>
	public ushort point;
}
