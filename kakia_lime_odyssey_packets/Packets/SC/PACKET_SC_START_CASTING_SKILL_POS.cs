/// <summary>
/// Server packet sent when an entity starts casting a skill targeting a position.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_START_CASTING_SKILL_POS
/// Size: 30 bytes
/// Ordinal: 2727
/// Initiates skill casting animation and cast bar targeting a world position.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_START_CASTING_SKILL_POS
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the caster</summary>
	public long fromInstID;

	/// <summary>Target position in world coordinates</summary>
	public FPOS pos;

	/// <summary>Skill type ID being cast</summary>
	public uint typeID;

	/// <summary>Total casting time in milliseconds</summary>
	public uint castTime;
}
