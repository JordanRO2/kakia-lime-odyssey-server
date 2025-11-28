/// <summary>
/// SC_ versions of skill packets without PACKET_FIX header.
/// These are used for server-side packet sending.
/// </summary>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Interface;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_SKILL_ADD (8 bytes total)
/// Sent when a character learns a new skill.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_SKILL_ADD : IPacketFixed
{
	/// <summary>Skill type ID being added</summary>
	public uint typeID;

	/// <summary>Initial level of the skill</summary>
	public ushort level;
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_SKILL_DEL (6 bytes total)
/// Sent when a skill is removed from the character's skill list.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_SKILL_DEL : IPacketFixed
{
	/// <summary>Skill type ID being removed</summary>
	public uint typeID;
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_SKILL_LV_UP (variable)
/// Sent when a skill's level increases.
/// Variable-length packet - followed by skill data.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_SKILL_LV_UP : IPacketVar
{
	// Variable length skill data follows
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_DELAYED_CASTING_SKILL (14 bytes total)
/// Updates the client on remaining cast time for a skill being cast.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_DELAYED_CASTING_SKILL : IPacketFixed
{
	/// <summary>Instance ID of the caster</summary>
	public long instID;

	/// <summary>Remaining casting time in milliseconds</summary>
	public uint remainTime;
}
