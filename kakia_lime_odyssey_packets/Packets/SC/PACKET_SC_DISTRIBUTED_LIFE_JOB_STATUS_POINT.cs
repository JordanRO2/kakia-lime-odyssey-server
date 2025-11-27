/// <summary>
/// Server packet sent after life job status points are distributed.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_DISTRIBUTED_LIFE_JOB_STATUS_POINT
/// Size: 12 bytes
/// Ordinal: 2596
/// Confirms the stat distribution and shows updated values.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_DISTRIBUTED_LIFE_JOB_STATUS_POINT
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Updated Idea stat value</summary>
	public ushort idea;

	/// <summary>Updated Sense stat value</summary>
	public ushort sense;

	/// <summary>Updated Mind stat value</summary>
	public ushort mind;

	/// <summary>Updated Craft stat value</summary>
	public ushort craft;

	/// <summary>Remaining status points</summary>
	public ushort point;
}
