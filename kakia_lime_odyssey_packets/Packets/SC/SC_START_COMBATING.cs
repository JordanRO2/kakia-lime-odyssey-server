using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Notifies client that an entity entered combat state
/// Verified against IDA: PACKET_SC_START_COMBATING (Size: 10 bytes)
/// Date: 2025-11-26
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_START_COMBATING : IPacketFixed
{
	/// <summary>
	/// Instance ID of the entity entering combat (0x2)
	/// IDA: __int64 instID
	/// </summary>
	public long instID;
}
