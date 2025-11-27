using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_SIT_DOWN - Server notifies that an entity has sat down
/// IDA Verification: PACKET_SC_SIT_DOWN
/// Size: 14 bytes (2 byte header + 8 byte objInstID + 4 byte tick)
/// Verified: 2025-11-26
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_SIT_DOWN : IPacketFixed
{
	public long objInstID;
	public uint tick;
}
