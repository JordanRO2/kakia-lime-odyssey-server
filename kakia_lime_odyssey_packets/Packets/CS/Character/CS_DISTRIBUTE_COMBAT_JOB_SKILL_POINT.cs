using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet to distribute combat job skill points.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_DISTRIBUTE_COMBAT_JOB_SKILL_POINT
/// Size: 8 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: unsigned int skillTypeID (4 bytes)
/// - 0x06: unsigned __int16 point (2 bytes)
/// Response: SC_SKILL_LV_UP
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_DISTRIBUTE_COMBAT_JOB_SKILL_POINT : IPacketFixed
{
	/// <summary>Skill type ID to level up (offset 0x02)</summary>
	public uint skillTypeID;

	/// <summary>Number of skill points to invest (offset 0x06)</summary>
	public ushort point;
}
