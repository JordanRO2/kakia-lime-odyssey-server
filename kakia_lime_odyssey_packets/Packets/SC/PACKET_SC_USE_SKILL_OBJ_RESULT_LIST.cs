/// <summary>
/// Server->Client: Skill use result when targeting an object/entity.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_USE_SKILL_OBJ_RESULT_LIST
/// Size: 36 bytes (4-byte header + 8-byte fromInstID + 8-byte toInstID + 4-byte typeID + 2-byte useHP + 2-byte useMP + 2-byte useSP + 2-byte useLP + 4-byte coolTime)
/// Sent when a skill is used on a target object (PC, NPC, monster, etc.).
/// Contains resource costs and cooldown information.
/// Variable-length packet - followed by array of APPLY_RESULT variants.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_USE_SKILL_OBJ_RESULT_LIST
{
	/// <summary>Variable-length packet header</summary>
	public PACKET_VAR header;

	/// <summary>Instance ID of the skill user</summary>
	public long fromInstID;

	/// <summary>Instance ID of the target object</summary>
	public long toInstID;

	/// <summary>Skill type ID from SkillInfo.xml</summary>
	public uint typeID;

	/// <summary>HP cost to use this skill</summary>
	public ushort useHP;

	/// <summary>MP cost to use this skill</summary>
	public ushort useMP;

	/// <summary>SP (Stamina Points) cost to use this skill</summary>
	public ushort useSP;

	/// <summary>LP (Life Points) cost to use this skill</summary>
	public ushort useLP;

	/// <summary>Cooldown time in milliseconds</summary>
	public uint coolTime;

	// Followed by variable-length array of APPLY_RESULT variants
}
