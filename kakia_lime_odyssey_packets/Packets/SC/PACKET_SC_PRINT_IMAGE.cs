/// <summary>
/// Server packet to display an image on screen.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_PRINT_IMAGE
/// Size: 10 bytes
/// Ordinal: 2867
/// Displays UI images, splash screens, or overlays with delay timing.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_PRINT_IMAGE
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Image ID to display</summary>
	public int ImageID;

	/// <summary>Delay time before showing image (in milliseconds)</summary>
	public uint delayTime;
}
