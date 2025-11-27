using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

/// <summary>
/// Mail list entry structure for displaying mail in the inbox list.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: POST_LIST
/// Size: 88 bytes
/// Memory Layout (IDA):
/// - 0x00: char[26] fromName (26 bytes)
/// - 0x1A: char[51] title (51 bytes)
/// - 0x50: int indexNumber (4 bytes)
/// - 0x54: bool isNew (1 byte)
/// Used in: SC_POST_LIST packet followed by array of POST_LIST entries
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct POST_LIST
{
	/// <summary>Sender's character name (max 25 chars + null terminator)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
	public byte[] fromName;

	/// <summary>Mail subject/title (max 50 chars + null terminator)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 51)]
	public byte[] title;

	/// <summary>Mail index number (1-based)</summary>
	public int indexNumber;

	/// <summary>Whether this mail is unread</summary>
	public bool isNew;
}
