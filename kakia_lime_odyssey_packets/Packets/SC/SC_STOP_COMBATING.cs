using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Notifies client that an entity exited combat state
/// Verified against IDA: PACKET_SC_STOP_COMBATING (Size: 10 bytes)
/// Date: 2025-11-26
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_STOP_COMBATING : IPacketFixed
{
	/// <summary>
	/// Instance ID of the entity leaving combat (0x2)
	/// IDA: __int64 instID
	/// </summary>
	public long instID;
}
