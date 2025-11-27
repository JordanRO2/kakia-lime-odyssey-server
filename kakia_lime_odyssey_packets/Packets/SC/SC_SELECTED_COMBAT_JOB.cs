/// <summary>
/// Server->Client packet broadcast when a player selects a combat job.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_SELECTED_COMBAT_JOB
/// Size: 9 bytes (11 with PACKET_FIX header)
/// Trigger: CS_CHOICED_COMBAT_JOB
/// </remarks>
using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_SELECTED_COMBAT_JOB : IPacketFixed
{
	/// <summary>Instance ID of the player who selected the job</summary>
	public long objInstID;

	/// <summary>Job type ID (combat job category) - char in IDA (signed)</summary>
	public sbyte jobTypeID;
}
