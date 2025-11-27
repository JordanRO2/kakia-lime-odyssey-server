/// <summary>
/// Client packet sent when player selects a target and requests to start player-to-player exchange.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_SELECT_TARGET_REQUEST_EXCHANGE
/// Size: 10 bytes
/// Ordinal: 2736
/// Combines target selection with exchange initiation request.
/// Note: Field name is objInstID not targetInstID in IDA.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_SELECT_TARGET_REQUEST_EXCHANGE
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the target player</summary>
	public long objInstID;
}
