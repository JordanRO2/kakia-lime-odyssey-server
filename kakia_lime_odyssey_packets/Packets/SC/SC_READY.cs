using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server confirms client is ready and provides zone and player region data.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_READY
/// Size: 334 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (ushort header) - 2 bytes (handled by IPacketFixed)
/// - 0x02: unsigned int zoneTypeID - 4 bytes
/// - 0x06: REGION_PC pc - 328 bytes
/// Triggered by: CS_READY from client
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_READY : IPacketFixed
{
	public uint zoneTypeID;
	public REGION_PC pc;
}
