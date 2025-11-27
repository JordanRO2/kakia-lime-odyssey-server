using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_START_CONTINUOUS_ACTION - Server notifies start of a continuous action (gathering, crafting, etc)
/// IDA Verification: PACKET_SC_START_CONTINUOUS_ACTION
/// Size: 14 bytes (2 byte header + 8 byte instID + 4 byte action)
/// Verified: 2025-11-26
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_START_CONTINUOUS_ACTION : IPacketFixed
{
	public long instID;
	public uint action;
}
