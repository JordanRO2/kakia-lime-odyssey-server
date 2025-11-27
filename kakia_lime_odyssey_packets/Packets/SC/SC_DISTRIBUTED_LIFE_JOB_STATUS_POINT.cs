/// <summary>
/// Server->Client packet confirming life job status point distribution.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_DISTRIBUTED_LIFE_JOB_STATUS_POINT
/// Size: 10 bytes (12 with PACKET_FIX header)
/// Triggered by: CS_DISTRIBUTE_LIFE_JOB_STATUS_POINT
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_DISTRIBUTED_LIFE_JOB_STATUS_POINT
{
	/// <summary>Total Idea stat after distribution</summary>
	public ushort idea;

	/// <summary>Total Sense stat after distribution</summary>
	public ushort sense;

	/// <summary>Total Mind stat after distribution</summary>
	public ushort mind;

	/// <summary>Total Craft stat after distribution</summary>
	public ushort craft;

	/// <summary>Remaining unallocated status points</summary>
	public ushort point;
}
