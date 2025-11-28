using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet to kick/ban a member from guild.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_GUILD_BAN
/// Size: 6 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: unsigned int idx (4 bytes)
/// Response: SC_GUILD_SECEDED (broadcast to guild)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_GUILD_BAN : IPacketFixed
{
	/// <summary>Guild member database index to kick (offset 0x02)</summary>
	public uint idx;
}
