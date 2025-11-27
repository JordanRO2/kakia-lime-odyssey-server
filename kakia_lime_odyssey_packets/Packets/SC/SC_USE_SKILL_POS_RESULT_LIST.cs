/// <summary>
/// Server->Client packet for skill used at a position result.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_USE_SKILL_POS_RESULT_LIST
/// Size: 40 bytes (4 byte PACKET_VAR header + 36 bytes payload)
/// Sent in response to: CS_USE_SKILL_POS
/// </remarks>
using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct SC_USE_SKILL_POS_RESULT_LIST : IPacketVar
{
	/// <summary>Caster object instance ID</summary>
	public long fromInstID;

	/// <summary>Target position (x, y, z)</summary>
	public FPOS toPos;

	/// <summary>Skill type ID</summary>
	public uint typeID;

	/// <summary>HP cost of the skill</summary>
	public ushort useHP;

	/// <summary>MP cost of the skill</summary>
	public ushort useMP;

	/// <summary>SP cost of the skill</summary>
	public ushort useSP;

	/// <summary>LP cost of the skill</summary>
	public ushort useLP;

	/// <summary>Cooldown time in milliseconds</summary>
	public uint coolTime;
}
