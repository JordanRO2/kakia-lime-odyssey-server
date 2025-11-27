using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Interface;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet to display an image on the client screen.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_PRINT_IMAGE
/// Size: 10 bytes
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes)
/// - 0x02: int ImageID (4 bytes) - Image resource ID to display
/// - 0x06: unsigned int delayTime (4 bytes) - Delay before showing image (milliseconds)
/// Used to display UI images, notifications, or visual overlays (death screen, level up, etc).
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_PRINT_IMAGE : IPacketFixed
{
	public int ImageID;
	public uint delayTime;
}
