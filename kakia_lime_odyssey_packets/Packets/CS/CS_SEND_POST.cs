using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// IDA Verified: 2025-11-26
/// Packet: PACKET_CS_SEND_POST @ 259 bytes
/// Client sends this to create and send a mail/post message.
/// Contains recipient name, title, up to 11 attachments, and variable length body text.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_SEND_POST
{
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
	public byte[] toName;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 51)]
	public byte[] title;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
	public POST_ATTACHING[] attaching;

	public ushort len;
}
