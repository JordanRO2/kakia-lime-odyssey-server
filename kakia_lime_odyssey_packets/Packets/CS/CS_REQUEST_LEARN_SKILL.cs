using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server: Request to learn a new skill from an NPC trainer.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_REQUEST_LEARN_SKILL
/// Size: 10 bytes total (2-byte header + 8-byte payload)
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: unsigned int typeID (4 bytes)
/// - 0x06: int lv (4 bytes)
/// Response: SC_SKILL_ADD on success
/// Note: Player requests to learn a skill from an NPC skill trainer
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_REQUEST_LEARN_SKILL : IPacketFixed
{
	/// <summary>Skill type ID to learn (from SkillInfo.xml)</summary>
	public uint typeID;

	/// <summary>Requested skill level to learn</summary>
	public int lv;
}
