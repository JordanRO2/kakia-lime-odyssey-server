using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet for skill used on object result.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_USE_SKILL_OBJ_RESULT_LIST
/// Size: 36 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_VAR header (4 bytes) - handled by IPacketVar
/// - 0x04: __int64 fromInstID (8 bytes)
/// - 0x0C: __int64 toInstID (8 bytes)
/// - 0x14: unsigned int typeID (4 bytes)
/// - 0x18: unsigned __int16 useHP (2 bytes)
/// - 0x1A: unsigned __int16 useMP (2 bytes)
/// - 0x1C: unsigned __int16 useSP (2 bytes)
/// - 0x1E: unsigned __int16 useLP (2 bytes)
/// - 0x20: unsigned int coolTime (4 bytes)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_USE_SKILL_OBJ_RESULT_LIST : IPacketVar
{
	/// <summary>Caster object instance ID (offset 0x04)</summary>
	public long fromInstID;

	/// <summary>Target object instance ID (offset 0x0C)</summary>
	public long toInstID;

	/// <summary>Skill type ID (offset 0x14)</summary>
	public uint typeID;

	/// <summary>HP cost of the skill (offset 0x18)</summary>
	public ushort useHP;

	/// <summary>MP cost of the skill (offset 0x1A)</summary>
	public ushort useMP;

	/// <summary>SP cost of the skill (offset 0x1C)</summary>
	public ushort useSP;

	/// <summary>LP cost of the skill (offset 0x1E)</summary>
	public ushort useLP;

	/// <summary>Cooldown time in milliseconds (offset 0x20)</summary>
	public uint coolTime;
}
