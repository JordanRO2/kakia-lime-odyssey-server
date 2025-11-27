using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_DO_ACTION - Server notifies that an entity performs an action (emote/animation)
/// IDA Verification: PACKET_SC_DO_ACTION
/// Size: 18 bytes (2 byte header + 8 byte objInstID + 4 byte type + 4 byte loopCount)
/// Verified: 2025-11-26
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_DO_ACTION : IPacketFixed
{
	public long objInstID;
	public uint type;
	public float loopCount;
}
