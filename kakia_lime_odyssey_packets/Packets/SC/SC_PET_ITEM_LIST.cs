using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet containing pet inventory items.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_PET_ITEM_LIST
/// Size: 8 bytes total (header + petObjSerial)
/// Memory Layout (IDA):
/// - 0x00: PACKET_VAR header (4 bytes) - handled by IPacketVar
/// - 0x04: int petObjSerial (4 bytes)
/// Note: Variable-length array of INVENTORY_ITEM follows
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_PET_ITEM_LIST : IPacketVar
{
	/// <summary>Pet object serial (offset 0x04)</summary>
	public int petObjSerial;

	// Note: Variable-length array of INVENTORY_ITEM follows, handled separately
}
