using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure Name: WIRED_EQUIPMENT
/// Size: 26 bytes (0x1A)
/// Represents equipment with wired slots (possibly for equipment linking/sets)
/// </summary>
/// <remarks>
/// Memory Layout (IDA):
/// - 0x00: EQUIPMENT (base structure) - 6 bytes
/// - 0x06: unsigned __int8[20] wiredSlot - 20 bytes
/// Total: 26 bytes
///
/// This structure extends the basic EQUIPMENT structure with wired slot information.
/// Wired slots may represent:
/// - Equipment set bonuses
/// - Linked/socketed items
/// - Equipment dependencies
/// - Slot states for connected equipment
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct WIRED_EQUIPMENT
{
	public EQUIPMENT baseEquipment;           // 6 bytes - base equipment data

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
	public byte[] wiredSlot;                  // 20 bytes - wired slot states
}
