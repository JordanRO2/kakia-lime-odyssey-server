using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet indicating a zone transfer is in progress.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_ZONE_TRANSFERING
/// Size: 18 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: unsigned int areaInstID (4 bytes)
/// - 0x06: FPOS pos (12 bytes)
/// Triggered By: Zone change events
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_ZONE_TRANSFERING : IPacketFixed
{
	/// <summary>Target area instance ID (offset 0x02)</summary>
	public uint areaInstID;

	/// <summary>Spawn position in target zone (offset 0x06)</summary>
	public FPOS pos;
}
