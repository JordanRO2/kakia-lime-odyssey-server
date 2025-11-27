using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet indicating a skill cast has started on a target object.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_START_CASTING_SKILL_OBJ
/// Size: 26 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: __int64 fromInstID (8 bytes)
/// - 0x0A: __int64 targetInstID (8 bytes)
/// - 0x12: unsigned int typeID (4 bytes)
/// - 0x16: unsigned int castTime (4 bytes)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_START_CASTING_SKILL_OBJ : IPacketFixed
{
	/// <summary>Caster object instance ID (offset 0x02)</summary>
	public long fromInstID;

	/// <summary>Target object instance ID (offset 0x0A)</summary>
	public long targetInstID;

	/// <summary>Skill type ID (offset 0x12)</summary>
	public uint typeID;

	/// <summary>Cast time in milliseconds (offset 0x16)</summary>
	public uint castTime;
}
