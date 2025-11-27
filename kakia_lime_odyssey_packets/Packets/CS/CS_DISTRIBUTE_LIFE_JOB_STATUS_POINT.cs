/// <summary>
/// Client->Server packet to distribute life job status points.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_DISTRIBUTE_LIFE_JOB_STATUS_POINT
/// Size: 8 bytes (10 with PACKET_FIX header)
/// Response: SC_DISTRIBUTED_LIFE_JOB_STATUS_POINT
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_DISTRIBUTE_LIFE_JOB_STATUS_POINT
{
	/// <summary>Points to add to Idea stat</summary>
	public ushort supplyedIdea;

	/// <summary>Points to add to Sense stat</summary>
	public ushort supplyedSense;

	/// <summary>Points to add to Mind stat</summary>
	public ushort supplyedMind;

	/// <summary>Points to add to Craft stat</summary>
	public ushort supplyedCraft;
}
