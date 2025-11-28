using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet to request trade with a targeted NPC.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_SELECT_TARGET_REQUEST_TRADE
/// Size: 10 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: __int64 objInstID (8 bytes)
/// Response: SC_TRADE_DESC
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_SELECT_TARGET_REQUEST_TRADE : IPacketFixed
{
	/// <summary>Object instance ID of the NPC merchant to trade with (offset 0x02)</summary>
	public long objInstID;
}
