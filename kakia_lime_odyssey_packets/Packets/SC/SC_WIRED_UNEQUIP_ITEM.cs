using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure Name: PACKET_SC_WIRED_UNEQUIP_ITEM
/// Size: 27 bytes (0x1B)
/// Server notifies client about forced unequip of an item with wired slot information.
/// Used when items are force-removed (e.g., death, durability zero, admin action).
/// </summary>
/// <remarks>
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX (base header) - 2 bytes [handled by IPacketFixed]
/// - 0x02: unsigned __int8 equipSlot - 1 byte (equipment slot being unequipped)
/// - 0x03: int invSlot - 4 bytes (destination inventory slot)
/// - 0x07: unsigned __int8[20] wiredSlot - 20 bytes (wired equipment slot states)
/// Total: 27 bytes
///
/// The wiredSlot array indicates which wired equipment slots are affected by this unequip.
/// Wired equipment appears to be a system where multiple items can be connected/linked.
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_WIRED_UNEQUIP_ITEM : IPacketFixed
{
	public byte equipSlot;                    // Offset: 0x02 - Equipment slot being unequipped
	public int invSlot;                       // Offset: 0x03 - Destination inventory slot

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
	public byte[] wiredSlot;                  // Offset: 0x07 - Wired equipment slot states (20 bytes)
}
