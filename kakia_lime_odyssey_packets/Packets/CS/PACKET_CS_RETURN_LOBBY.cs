/// <summary>
/// Client packet requesting to return to the character selection lobby.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_RETURN_LOBBY
/// Size: 2 bytes
/// Ordinal: 2452
/// Simple request packet to exit the game world and return to character selection.
/// Server responds with SC_REENTER_LOBBY.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_RETURN_LOBBY
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;
}
