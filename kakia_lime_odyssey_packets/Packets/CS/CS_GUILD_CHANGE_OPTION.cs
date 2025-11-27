using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet to change guild options/settings.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_GUILD_CHANGE_OPTION
/// Size: 3 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: unsigned char type (1 byte)
/// Response: SC_GUILD_CHANGED_OPTION (broadcast to guild)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_GUILD_CHANGE_OPTION : IPacketFixed
{
	/// <summary>Guild option type/flags (offset 0x02)</summary>
	public byte type;
}
