/// <summary>
/// Client packet sent when player readies their weapon for melee combat.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_READY_WEAPON_HITING
/// Size: 2 bytes
/// Ordinal: 2553
/// Simple notification that the player has readied their weapon with no target.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_READY_WEAPON_HITING
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;
}
