/// <summary>
/// Server->Client: New skill learned/added to character.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_SKILL_ADD
/// Size: 8 bytes (2-byte header + 4-byte typeID + 2-byte level)
/// Sent when a character learns a new skill from a trainer or gains it through other means.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_SKILL_ADD : IPacketFixed
{
	/// <summary>Skill type ID being added</summary>
	public uint typeID;

	/// <summary>Initial level of the skill</summary>
	public ushort level;
}
