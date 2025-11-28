using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet sent when player opens their bank storage.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_BANK_OPENED
/// Size: 8 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_VAR header (4 bytes) - handled by IPacketVar
/// - 0x04: int maxCount (4 bytes)
/// Note: Actual items follow in separate INVENTORY_ITEM packets
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_BANK_OPENED : IPacketVar
{
	/// <summary>Maximum bank slot count (offset 0x04)</summary>
	public int maxCount;
}
