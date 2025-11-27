/// <summary>
/// Server->Client: Skill removed from character.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_SKILL_DEL
/// Size: 6 bytes (2-byte header + 4-byte typeID)
/// Sent when a skill is removed from the character's skill list.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_SKILL_DEL
{
	/// <summary>Fixed-length packet header</summary>
	public PACKET_FIX header;

	/// <summary>Skill type ID being removed</summary>
	public uint typeID;
}
