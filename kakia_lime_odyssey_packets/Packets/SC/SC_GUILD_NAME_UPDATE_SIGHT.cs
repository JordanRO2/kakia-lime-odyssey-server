using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet updating guild name visibility for a character in sight.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_GUILD_NAME_UPDATE_SIGHT
/// Size: 61 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: char[51] name (51 bytes)
/// - 0x35: __int64 instID (8 bytes)
/// Triggered by: Character enters sight with guild tag
/// Sent to: Players who can see the character
/// Note: No padding between name and instID (offset 0x35 = 2 + 51 = 53)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_GUILD_NAME_UPDATE_SIGHT : IPacketFixed
{
	/// <summary>Guild name (offset 0x02, 51 bytes)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 51)]
	public byte[] name;

	/// <summary>Character instance ID (offset 0x35)</summary>
	public long instID;
}
