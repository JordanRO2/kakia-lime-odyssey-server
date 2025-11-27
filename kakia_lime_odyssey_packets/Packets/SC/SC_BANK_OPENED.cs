using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_BANK_OPENED @ 8 bytes
/// Sent when player opens their bank storage.
/// Contains bank capacity. Actual items follow in separate INVENTORY_ITEM packets.
/// This is a variable-length packet (PACKET_VAR header).
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_BANK_OPENED
{
	public int maxCount;
}
