using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet for skill used at a position result.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_USE_SKILL_POS_RESULT_LIST
/// Size: 40 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_VAR header (4 bytes) - handled by IPacketVar
/// - 0x04: __int64 fromInstID (8 bytes)
/// - 0x0C: FPOS toPos (12 bytes)
/// - 0x18: unsigned int typeID (4 bytes)
/// - 0x1C: unsigned __int16 useHP (2 bytes)
/// - 0x1E: unsigned __int16 useMP (2 bytes)
/// - 0x20: unsigned __int16 useSP (2 bytes)
/// - 0x22: unsigned __int16 useLP (2 bytes)
/// - 0x24: unsigned int coolTime (4 bytes)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_USE_SKILL_POS_RESULT_LIST : IPacketVar
{
	/// <summary>Caster object instance ID (offset 0x04)</summary>
	public long fromInstID;

	/// <summary>Target position (offset 0x0C)</summary>
	public FPOS toPos;

	/// <summary>Skill type ID (offset 0x18)</summary>
	public uint typeID;

	/// <summary>HP cost of the skill (offset 0x1C)</summary>
	public ushort useHP;

	/// <summary>MP cost of the skill (offset 0x1E)</summary>
	public ushort useMP;

	/// <summary>SP cost of the skill (offset 0x20)</summary>
	public ushort useSP;

	/// <summary>LP cost of the skill (offset 0x22)</summary>
	public ushort useLP;

	/// <summary>Cooldown time in milliseconds (offset 0x24)</summary>
	public uint coolTime;
}
