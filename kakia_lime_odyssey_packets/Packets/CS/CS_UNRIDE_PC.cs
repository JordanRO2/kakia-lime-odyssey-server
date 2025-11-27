/// <summary>
/// Client->Server packet to dismount from a mount/pet.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_CS_UNRIDE_PC
/// Size: 2 bytes (PACKET_FIX header only)
/// Response: SC_UNRIDE_PC
/// This packet contains no additional fields beyond the header.
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_UNRIDE_PC
{
	// No additional fields - header only packet
}
