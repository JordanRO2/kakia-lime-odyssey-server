using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client update of party member's state (HP, MP, position, etc).
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_PARTY_MEMBER_STATE
/// Size: 58 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: unsigned int idx (4 bytes)
/// - 0x06: PARTY_MEMBER_STATE state (52 bytes)
/// Triggered by: Member state changes
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_PARTY_MEMBER_STATE : IPacketFixed
{
	/// <summary>Party member index (offset 0x02)</summary>
	public uint idx;

	/// <summary>Updated state information (offset 0x06)</summary>
	public PARTY_MEMBER_STATE state;
}
