/// <summary>
/// Server->Client packet to indicate skill casting started at a position.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_START_CASTING_SKILL_POS
/// Size: 30 bytes (2 byte PACKET_FIX header + 28 bytes payload)
/// Sent when: Player begins casting a ground-targeted skill with cast time
/// </remarks>
using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_START_CASTING_SKILL_POS : IPacketFixed
{
	/// <summary>Caster object instance ID</summary>
	public long fromInstID;

	/// <summary>Target position (x, y, z)</summary>
	public FPOS toPos;

	/// <summary>Skill type ID being cast</summary>
	public uint typeID;

	/// <summary>Cast time in milliseconds</summary>
	public uint castTime;
}
