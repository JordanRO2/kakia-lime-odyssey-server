using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet to request common status of an object.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_REQUEST_COMMON_STATUS
/// Size: 10 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: __int64 objInstID (8 bytes)
/// Response: SC_COMMON_STATUS
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_REQUEST_COMMON_STATUS : IPacketFixed
{
	/// <summary>Object instance ID to get status for (offset 0x02)</summary>
	public long objInstID;
}
