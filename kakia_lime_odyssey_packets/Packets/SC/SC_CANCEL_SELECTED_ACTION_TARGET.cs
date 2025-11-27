using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_CANCEL_SELECTED_ACTION_TARGET - Server notifies that an entity cancelled their action target
/// IDA Verification: PACKET_SC_CANCEL_SELECTED_ACTION_TARGET
/// Size: 10 bytes (2 byte header + 8 byte objInstID)
/// Verified: 2025-11-26
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_CANCEL_SELECTED_ACTION_TARGET : IPacketFixed
{
	public long objInstID;
}
