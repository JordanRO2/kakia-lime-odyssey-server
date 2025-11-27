using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet to dismount from a mount/pet.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_UNRIDE_PC
/// Size: 2 bytes total (header only)
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// Response: SC_UNRIDE_PC
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_UNRIDE_PC : IPacketFixed
{
	// Header only - no payload
}
