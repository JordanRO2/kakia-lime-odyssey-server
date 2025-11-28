using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client notification that an entity performs an action (emote/animation).
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_DO_ACTION
/// Size: 18 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: __int64 objInstID (8 bytes)
/// - 0x0A: unsigned int type (4 bytes)
/// - 0x0E: float loopCount (4 bytes)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_DO_ACTION : IPacketFixed
{
	/// <summary>Object instance ID performing the action (offset 0x02)</summary>
	public long objInstID;

	/// <summary>Action type ID (offset 0x0A)</summary>
	public uint type;

	/// <summary>Number of times to loop the action (offset 0x0E)</summary>
	public float loopCount;
}
