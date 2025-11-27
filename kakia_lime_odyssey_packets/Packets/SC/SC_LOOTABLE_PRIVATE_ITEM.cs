/// <summary>
/// Server->Client notification that an object has private loot available (loot reserved for specific player).
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_LOOTABLE_PRIVATE_ITEM
/// Size: 10 bytes (8 bytes + 2 byte PACKET_FIX header)
/// Fields:
///   - objInstID: Instance ID of the lootable object with private loot (8 bytes at offset 0x02)
/// Triggered by: Mob death with private/personal loot enabled
/// </remarks>
using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_LOOTABLE_PRIVATE_ITEM : IPacketFixed
{
	/// <summary>Instance ID of the object with private loot</summary>
	public long objInstID;
}
