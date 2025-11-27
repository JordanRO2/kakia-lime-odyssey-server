/// <summary>
/// Server->Client packet indicating a zone transfer is in progress.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_ZONE_TRANSFERING
/// Size: 16 bytes (18 with PACKET_FIX header)
/// Triggered By: Zone change events
/// Fields:
/// - areaInstID: Target area instance ID (4 bytes, unsigned int)
/// - pos: Spawn position in target zone (12 bytes, FPOS struct)
/// </remarks>
using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_ZONE_TRANSFERING : IPacketFixed
{
	/// <summary>Target area instance ID</summary>
	public uint areaInstID;

	/// <summary>Spawn position in target zone</summary>
	public FPOS pos;
}
