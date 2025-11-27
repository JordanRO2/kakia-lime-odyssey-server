/// <summary>
/// Client packet sent when player selects an NPC and requests to start trading.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_SELECT_TARGET_REQUEST_TRADE
/// Size: 10 bytes
/// Ordinal: 2759
/// Combines NPC selection with trade window initiation request.
/// Note: Field name is objInstID not targetInstID in IDA.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_SELECT_TARGET_REQUEST_TRADE
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the target NPC merchant</summary>
	public long objInstID;
}
