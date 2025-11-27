using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_LEAVE_ZONEOBJ (10 bytes)
/// Layout:
///   - PACKET_FIX header (2 bytes, offset 0x00) - handled by IPacketFixed
///   - __int64 objInstID (8 bytes, offset 0x02)
/// Purpose: Base structure for all zone object leave sight packets
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct SC_LEAVE_ZONEOBJ : IPacketFixed
{
	public long objInstID;
}
