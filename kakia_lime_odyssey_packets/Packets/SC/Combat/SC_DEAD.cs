using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_DEAD @ 10 bytes
/// Sent when an object (player, monster, NPC) dies.
/// Notifies client that the object with the given instance ID has died.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_DEAD : IPacketFixed
{
	/// <summary>Instance ID of the object that died</summary>
	public long objInstID;
}
