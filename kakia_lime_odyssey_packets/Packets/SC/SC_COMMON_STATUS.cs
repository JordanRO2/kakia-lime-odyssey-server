using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet with common status update for an object.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_COMMON_STATUS
/// Size: 30 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: __int64 objInstID (8 bytes)
/// - 0x0A: COMMON_STATUS status (20 bytes)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_COMMON_STATUS : IPacketFixed
{
	/// <summary>Object instance ID (offset 0x02)</summary>
	public long objInstID;

	/// <summary>Common status data (offset 0x0A)</summary>
	public COMMON_STATUS status;
}
