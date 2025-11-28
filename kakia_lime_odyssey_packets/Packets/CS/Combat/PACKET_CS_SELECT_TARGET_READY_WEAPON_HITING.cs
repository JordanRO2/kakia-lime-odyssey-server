/// <summary>
/// Client packet sent when player readies their weapon targeting a specific entity.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_SELECT_TARGET_READY_WEAPON_HITING
/// Size: 10 bytes
/// Ordinal: 2552
/// Readies weapon with a specific target selected.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_SELECT_TARGET_READY_WEAPON_HITING
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the target entity</summary>
	public long targetInstID;
}
