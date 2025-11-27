using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Notifies client that they received a guild invitation.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_GUILD_INVITED
/// Size: 79 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: char[26] pcName (26 bytes) - Inviter's character name
/// - 0x1C: char[51] guildName (51 bytes) - Guild name
/// Triggered by: CS_GUILD_INVITE from another player
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_GUILD_INVITED : IPacketFixed
{
	/// <summary>Inviter's character name (offset 0x02, 26 bytes)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
	public byte[] pcName;

	/// <summary>Guild name (offset 0x1C, 51 bytes)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 51)]
	public byte[] guildName;
}
