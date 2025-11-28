using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client notification that private loot on an object is no longer available.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_DISABLED_LOOTING_PRIVATE_ITEM
/// Size: 10 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: __int64 objInstID (8 bytes)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_DISABLED_LOOTING_PRIVATE_ITEM : IPacketFixed
{
	/// <summary>Instance ID of the object whose private loot expired (offset 0x02)</summary>
	public long objInstID;
}
