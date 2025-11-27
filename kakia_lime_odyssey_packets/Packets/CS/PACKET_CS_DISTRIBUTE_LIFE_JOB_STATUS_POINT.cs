/// <summary>
/// Client packet to distribute life job status points.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_DISTRIBUTE_LIFE_JOB_STATUS_POINT
/// Size: 10 bytes
/// Ordinal: 2608
/// Allows player to allocate life job stat points to Idea, Sense, Mind, Craft.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_DISTRIBUTE_LIFE_JOB_STATUS_POINT
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Points to distribute to Idea stat</summary>
	public ushort supplyedIdea;

	/// <summary>Points to distribute to Sense stat</summary>
	public ushort supplyedSense;

	/// <summary>Points to distribute to Mind stat</summary>
	public ushort supplyedMind;

	/// <summary>Points to distribute to Craft stat</summary>
	public ushort supplyedCraft;
}
