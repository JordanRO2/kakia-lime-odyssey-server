using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: INVENTORY_ITEM @ 336 bytes
/// Represents a single inventory item with all its properties and inherited stats.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct INVENTORY_ITEM
{
	public int slot;
	public int typeID;
	public long count;
	public int durability;
	public int mdurability;
	public int remainExpiryTime;
	public int grade;
	public ITEM_INHERITS inherits;
}
