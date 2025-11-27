using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Notifies client that they received a party invitation.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_PARTY_INVITED
/// Size: 69 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: char[26] pcName (26 bytes) - Inviter's character name
/// - 0x1C: char[41] partyName (41 bytes) - Party name
/// Triggered by: CS_PARTY_INVITE from another player
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_PARTY_INVITED : IPacketFixed
{
	/// <summary>Inviter's character name (offset 0x02, 26 bytes)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
	public byte[] pcName;

	/// <summary>Party name (offset 0x1C, 41 bytes)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 41)]
	public byte[] partyName;
}
