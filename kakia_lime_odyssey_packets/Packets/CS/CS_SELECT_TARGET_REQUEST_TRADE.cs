/// <summary>
/// Client->Server packet to request trade with a targeted NPC.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_SELECT_TARGET_REQUEST_TRADE
/// Size: 8 bytes (10 with PACKET_FIX header)
/// Response: SC_TRADE_DESC
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_SELECT_TARGET_REQUEST_TRADE
{
	/// <summary>Object instance ID of the NPC merchant to trade with</summary>
	public long objInstID;
}
