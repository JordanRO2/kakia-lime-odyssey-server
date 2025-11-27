using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// IDA Verified: 2025-11-26
/// Packet: PACKET_SC_POST @ 3649 bytes
/// Server sends this with complete mail/post message details.
/// Contains sender name, title, and up to 11 attached items with full stats.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_POST
{
	public int indexNumber;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
	public byte[] fromName;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 51)]
	public byte[] title;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
	public POST_ATTACHED[] attached;
}
