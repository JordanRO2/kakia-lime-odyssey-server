/// <summary>
/// Structure representing a slot in the stuff/material making (gathering) interface.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: STUFF_MAKE_SLOT
/// Size: 16 bytes
/// Used for adding/removing items from the gathering crafting queue.
/// Note: 4-byte padding between slot and count fields (offset 0x04-0x07).
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Common;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct STUFF_MAKE_SLOT
{
	/// <summary>Inventory slot index</summary>
	public int slot;

	/// <summary>Padding to match IDA structure alignment</summary>
	private int padding;

	/// <summary>Quantity of items to process</summary>
	public long count;
}
