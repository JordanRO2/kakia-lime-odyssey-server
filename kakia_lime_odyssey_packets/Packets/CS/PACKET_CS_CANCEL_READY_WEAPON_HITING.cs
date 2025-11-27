/// <summary>
/// Client packet sent when player cancels weapon ready state.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_CANCEL_READY_WEAPON_HITING
/// Size: 2 bytes
/// Ordinal: 2554
/// Returns weapon to unready/sheathed state.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_CANCEL_READY_WEAPON_HITING
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;
}
