using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_EQUIP_ITEM_LIST @ 4 bytes (header only)
/// This is a variable-length packet (PACKET_VAR header) that sends equipped items list.
///
/// IDA Structure Details:
/// - Base structure: 4 bytes (PACKET_VAR header with size/count)
/// - Variable data: Array of EQUIP_ITEM (320 bytes each)
///
/// Usage: Sent when player logs in or equips/unequips items to sync equipment state.
/// Related to CS_START_GAME - this is typically sent during character loading.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_EQUIP_ITEM_LIST
{
	// Variable-length array of equipped items
	// Each element is 320 bytes (EQUIP_ITEM structure)
	// The count is stored in the PACKET_VAR header
	public EQUIP_ITEM[] equipList;
}
