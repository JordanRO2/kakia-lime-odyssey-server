using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet confirming life job status point distribution.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_DISTRIBUTED_LIFE_JOB_STATUS_POINT
/// Size: 12 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: unsigned __int16 idea (2 bytes)
/// - 0x04: unsigned __int16 sense (2 bytes)
/// - 0x06: unsigned __int16 mind (2 bytes)
/// - 0x08: unsigned __int16 craft (2 bytes)
/// - 0x0A: unsigned __int16 point (2 bytes)
/// Triggered by: CS_DISTRIBUTE_LIFE_JOB_STATUS_POINT
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_DISTRIBUTED_LIFE_JOB_STATUS_POINT : IPacketFixed
{
	/// <summary>Total Idea stat after distribution (offset 0x02)</summary>
	public ushort idea;

	/// <summary>Total Sense stat after distribution (offset 0x04)</summary>
	public ushort sense;

	/// <summary>Total Mind stat after distribution (offset 0x06)</summary>
	public ushort mind;

	/// <summary>Total Craft stat after distribution (offset 0x08)</summary>
	public ushort craft;

	/// <summary>Remaining unallocated status points (offset 0x0A)</summary>
	public ushort point;
}
