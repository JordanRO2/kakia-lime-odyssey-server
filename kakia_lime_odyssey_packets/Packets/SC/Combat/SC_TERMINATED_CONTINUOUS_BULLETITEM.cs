using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet indicating a continuous bullet item was terminated.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_TERMINATED_CONTINUOUS_BULLETITEM
/// Size: 10 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: __int64 bulletInstID (8 bytes)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_TERMINATED_CONTINUOUS_BULLETITEM : IPacketFixed
{
	/// <summary>Bullet instance ID that was terminated (offset 0x02)</summary>
	public long bulletInstID;
}
