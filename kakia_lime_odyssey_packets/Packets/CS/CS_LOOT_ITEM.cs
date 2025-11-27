/// <summary>
/// Client->Server request to loot an item from a lootable container.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_CS_LOOT_ITEM
/// Size: 18 bytes (16 bytes + 2 byte PACKET_FIX header)
/// Fields:
///   - popSlot: Source slot in loot window (4 bytes at offset 0x02)
///   - popCount: Amount to loot (8 bytes at offset 0x06)
///   - pushSlot: Destination slot in inventory (4 bytes at offset 0x0E)
/// Response: SC_LOOTED_ITEM
/// </remarks>
using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_LOOT_ITEM : IPacketFixed
{
	/// <summary>Source slot in loot window</summary>
	public int popSlot;

	/// <summary>Amount to loot</summary>
	public long popCount;

	/// <summary>Destination slot in player inventory</summary>
	public int pushSlot;
}
