/// <summary>
/// Client->Server: Request to learn a new skill from an NPC trainer.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_REQUEST_LEARN_SKILL
/// Size: 10 bytes (2-byte header + 4-byte typeID + 4-byte level)
/// Response: SC_SKILL_ADD on success
/// Note: Player requests to learn a skill from an NPC skill trainer
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_REQUEST_LEARN_SKILL
{
	/// <summary>Skill type ID to learn (from SkillInfo.xml)</summary>
	public uint typeID;

	/// <summary>Requested skill level to learn</summary>
	public int lv;
}
