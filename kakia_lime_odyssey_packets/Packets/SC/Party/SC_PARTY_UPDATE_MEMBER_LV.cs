using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client update of party member's level and max stats.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_PARTY_UPDATE_MEMBER_LV
/// Size: 26 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: unsigned int idx (4 bytes)
/// - 0x06: int lv (4 bytes)
/// - 0x0A: int mhp (4 bytes)
/// - 0x0E: int mmp (4 bytes)
/// - 0x12: int mlp (4 bytes)
/// - 0x16: int msp (4 bytes)
/// Triggered by: Member level up
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_PARTY_UPDATE_MEMBER_LV : IPacketFixed
{
	/// <summary>Party member index (offset 0x02)</summary>
	public uint idx;

	/// <summary>New level (offset 0x06)</summary>
	public int lv;

	/// <summary>New maximum HP (offset 0x0A)</summary>
	public int mhp;

	/// <summary>New maximum MP (offset 0x0E)</summary>
	public int mmp;

	/// <summary>New maximum LP (offset 0x12)</summary>
	public int mlp;

	/// <summary>New maximum SP (Stamina Points) (offset 0x16)</summary>
	public int msp;
}
