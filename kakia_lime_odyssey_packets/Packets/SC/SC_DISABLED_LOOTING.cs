/// <summary>
/// Server->Client notification that a lootable object is no longer available for looting.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_DISABLED_LOOTING
/// Size: 10 bytes (8 bytes + 2 byte PACKET_FIX header)
/// Fields:
///   - objInstID: Instance ID of the object that is no longer lootable (8 bytes at offset 0x02)
/// Triggered by: Loot timer expiration or all items looted
/// </remarks>
using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_DISABLED_LOOTING : IPacketFixed
{
	/// <summary>Instance ID of the object that is no longer lootable</summary>
	public long objInstID;
}
