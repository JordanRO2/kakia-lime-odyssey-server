/// <summary>
/// Structure representing an item slot with type and index.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: ITEM_SLOT
/// Size: 5 bytes
/// Used to identify items by slot type (inventory, equipment, etc.) and index.
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Common;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ITEM_SLOT
{
	/// <summary>Slot type (0=inventory, 1=equipment, etc.)</summary>
	public byte type;

	/// <summary>Slot index within the specified type</summary>
	public int slot;
}
