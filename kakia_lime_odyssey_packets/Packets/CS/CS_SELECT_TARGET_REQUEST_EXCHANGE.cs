using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client sends this packet when requesting to start a trade exchange with another player.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_SELECT_TARGET_REQUEST_EXCHANGE
/// Size: 10 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: __int64 objInstID (8 bytes) - target player instance ID
/// Use case: Initiating a trade request with another player
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_SELECT_TARGET_REQUEST_EXCHANGE : IPacketFixed
{
	public long objInstID;
}
