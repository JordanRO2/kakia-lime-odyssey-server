using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// IDA Verified: 2025-11-27
/// Packet: PACKET_CS_SEND_POST @ 259 bytes base (4 byte PACKET_VAR header + 255 byte payload)
/// Client sends this to create and send a mail/post message.
/// Contains recipient name, title, up to 11 attachments, and variable length body text.
/// Variable-length packet: body text follows after this struct.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_SEND_POST : IPacketVar
{
	/// <summary>Recipient character name (26 bytes)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
	public byte[] toName;

	/// <summary>Mail title (51 bytes)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 51)]
	public byte[] title;

	/// <summary>Attached items (11 slots)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
	public POST_ATTACHING[] attaching;

	/// <summary>Length of the body text that follows</summary>
	public ushort len;
}
