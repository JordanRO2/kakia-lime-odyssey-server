using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client update of party member's HP.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_PARTY_UPDATE_MEMBER_HP
/// Size: 14 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: unsigned int idx (4 bytes)
/// - 0x06: int hp (4 bytes)
/// - 0x0A: int mhp (4 bytes)
/// Triggered by: Member HP change
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_PARTY_UPDATE_MEMBER_HP : IPacketFixed
{
	/// <summary>Party member index (offset 0x02)</summary>
	public uint idx;

	/// <summary>Current HP (offset 0x06)</summary>
	public int hp;

	/// <summary>Maximum HP (offset 0x0A)</summary>
	public int mhp;
}
