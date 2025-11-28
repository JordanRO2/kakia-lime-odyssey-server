using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Contains complete guild information sent to a member.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_GUILD_INFO
/// Size: 208 bytes total (variable-length packet)
/// Memory Layout (IDA):
/// - 0x00: PACKET_VAR header (4 bytes) - handled by IPacketVar
/// - 0x04: unsigned int myID (4 bytes) - Player's member ID in guild
/// - 0x08: GUILD guildInfo (200 bytes) - Guild information
/// Triggered by: Guild join, guild info request
/// Note: GUILD_MEMBER array may follow in variable data
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_GUILD_INFO : IPacketVar
{
	/// <summary>Player's member ID in the guild (offset 0x04)</summary>
	public uint myID;

	/// <summary>Guild information (offset 0x08)</summary>
	public GUILD guildInfo;
}
