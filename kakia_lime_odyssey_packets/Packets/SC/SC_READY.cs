using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet confirming client is ready with zone and player data.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_READY
/// Size: 334 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: unsigned int zoneTypeID (4 bytes)
/// - 0x06: REGION_PC pc (328 bytes)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_READY : IPacketFixed
{
	/// <summary>Zone type ID (offset 0x02)</summary>
	public uint zoneTypeID;

	/// <summary>Player region data (offset 0x06)</summary>
	public REGION_PC pc;
}
