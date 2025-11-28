using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client update of party member's position.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_PARTY_UPDATE_MEMBER_POS
/// Size: 22 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: unsigned int idx (4 bytes)
/// - 0x06: unsigned int zoneID (4 bytes)
/// - 0x0A: FPOS pos (12 bytes)
/// Triggered by: Member moves to new zone or significant position change
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_PARTY_UPDATE_MEMBER_POS : IPacketFixed
{
	/// <summary>Party member index (offset 0x02)</summary>
	public uint idx;

	/// <summary>Zone ID where member is located (offset 0x06)</summary>
	public uint zoneID;

	/// <summary>Position in world (offset 0x0A)</summary>
	public FPOS pos;
}
