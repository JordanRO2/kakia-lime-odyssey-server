/// <summary>
/// Server->Client notification that private loot on an object is no longer available.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_DISABLED_LOOTING_PRIVATE_ITEM
/// Size: 10 bytes (8 bytes + 2 byte PACKET_FIX header)
/// Fields:
///   - objInstID: Instance ID of the object whose private loot expired (8 bytes at offset 0x02)
/// Triggered by: Private loot timer expiration
/// </remarks>
using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_DISABLED_LOOTING_PRIVATE_ITEM : IPacketFixed
{
	/// <summary>Instance ID of the object whose private loot expired</summary>
	public long objInstID;
}
