using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet to distribute life job status points.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_DISTRIBUTE_LIFE_JOB_STATUS_POINT
/// Size: 10 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: unsigned short supplyedIdea (2 bytes)
/// - 0x04: unsigned short supplyedSense (2 bytes)
/// - 0x06: unsigned short supplyedMind (2 bytes)
/// - 0x08: unsigned short supplyedCraft (2 bytes)
/// Response: SC_DISTRIBUTED_LIFE_JOB_STATUS_POINT
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_DISTRIBUTE_LIFE_JOB_STATUS_POINT : IPacketFixed
{
	/// <summary>Points to add to Idea stat (offset 0x02)</summary>
	public ushort supplyedIdea;

	/// <summary>Points to add to Sense stat (offset 0x04)</summary>
	public ushort supplyedSense;

	/// <summary>Points to add to Mind stat (offset 0x06)</summary>
	public ushort supplyedMind;

	/// <summary>Points to add to Craft stat (offset 0x08)</summary>
	public ushort supplyedCraft;
}
