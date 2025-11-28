using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet with complete mail/post message details.
/// Contains sender name, title, and up to 11 attached items with full stats.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_POST
/// Size: 3649 bytes total (4-byte header + 3645-byte payload)
/// Memory Layout (IDA):
/// - 0x00: PACKET_VAR header (4 bytes) - handled by IPacketVar
/// - 0x04: int indexNumber (4 bytes)
/// - 0x08: char[26] fromName (26 bytes)
/// - 0x22: char[51] title (51 bytes)
/// - 0x55: POST_ATTACHED[11] attached (3564 bytes = 324 * 11)
/// Followed by: variable length message body
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_POST : IPacketVar
{
	/// <summary>Mail index number (1-based)</summary>
	public int indexNumber;

	/// <summary>Sender's character name (max 25 chars + null terminator)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
	public byte[] fromName;

	/// <summary>Mail subject/title (max 50 chars + null terminator)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 51)]
	public byte[] title;

	/// <summary>Attached items (up to 11 items with full stats)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
	public POST_ATTACHED[] attached;
}
