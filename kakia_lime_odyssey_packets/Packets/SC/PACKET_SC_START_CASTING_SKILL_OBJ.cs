/// <summary>
/// Server packet sent when an entity starts casting a skill targeting another object.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_START_CASTING_SKILL_OBJ
/// Size: 26 bytes
/// Ordinal: 2726
/// Initiates skill casting animation and cast bar targeting an object.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_START_CASTING_SKILL_OBJ
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the caster</summary>
	public long fromInstID;

	/// <summary>Instance ID of the target object</summary>
	public long targetInstID;

	/// <summary>Skill type ID being cast</summary>
	public uint typeID;

	/// <summary>Total casting time in milliseconds</summary>
	public uint castTime;
}
