/// <summary>
/// Client packet sent when player selects an action target.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_SELECT_ACTION_TARGET
/// Size: 10 bytes
/// Ordinal: 2499
/// Used to select a target for combat or interaction.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_SELECT_ACTION_TARGET
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the target entity</summary>
	public long targetInstID;
}
