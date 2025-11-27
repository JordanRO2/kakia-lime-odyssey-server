using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet to indicate skill casting started at a position.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_START_CASTING_SKILL_POS
/// Size: 30 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: __int64 fromInstID (8 bytes)
/// - 0x0A: FPOS pos (12 bytes)
/// - 0x16: unsigned int typeID (4 bytes)
/// - 0x1A: unsigned int castTime (4 bytes)
/// Triggered by: Player begins casting ground-targeted skill
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_START_CASTING_SKILL_POS : IPacketFixed
{
	/// <summary>Caster object instance ID (offset 0x02)</summary>
	public long fromInstID;

	/// <summary>Target position (offset 0x0A)</summary>
	public FPOS pos;

	/// <summary>Skill type ID being cast (offset 0x16)</summary>
	public uint typeID;

	/// <summary>Cast time in milliseconds (offset 0x1A)</summary>
	public uint castTime;
}
