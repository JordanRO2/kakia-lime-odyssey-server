using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Notifies client that a member has left the party.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_PARTY_SECEDED
/// Size: 6 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: unsigned int idx (4 bytes) - Member index who left
/// Triggered by: CS_PARTY_SECEDE, member logout
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_PARTY_SECEDED : IPacketFixed
{
	/// <summary>Party member index who left (offset 0x02)</summary>
	public uint idx;
}
