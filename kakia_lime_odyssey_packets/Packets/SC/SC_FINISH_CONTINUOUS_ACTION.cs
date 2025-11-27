using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_FINISH_CONTINUOUS_ACTION - Server notifies end of a continuous action
/// IDA Verification: PACKET_SC_FINISH_CONTINUOUS_ACTION
/// Size: 10 bytes (2 byte header + 8 byte instID)
/// Verified: 2025-11-26
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_FINISH_CONTINUOUS_ACTION : IPacketFixed
{
	public long instID;
}