using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

/// <summary>
/// Represents an item slot location in inventory/equipment/bank.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: ITEM_SLOT
/// Size: 5 bytes
/// Memory Layout (IDA):
/// - 0x00: unsigned __int8 type (1 byte) - Slot type (inventory=0, equipment=1, bank=2, etc)
/// - 0x01: int slot (4 bytes) - Slot index within the container
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ITEM_SLOT
{
	/// <summary>Slot container type (offset 0x00)</summary>
	public byte type;

	/// <summary>Slot index within the container (offset 0x01)</summary>
	public int slot;
}
