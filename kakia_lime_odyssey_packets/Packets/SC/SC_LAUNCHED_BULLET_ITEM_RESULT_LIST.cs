using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet containing bullet item hit results.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_LAUNCHED_BULLET_ITEM_RESULT_LIST
/// Size: 20 bytes total (variable-length packet)
/// Memory Layout (IDA):
/// - 0x00: PACKET_VAR header (4 bytes) - handled by IPacketVar
/// - 0x04: int typeID (4 bytes)
/// - 0x08: __int64 from (8 bytes)
/// - 0x10: unsigned int tick (4 bytes)
/// Note: Variable-length - hit result data follows
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_LAUNCHED_BULLET_ITEM_RESULT_LIST : IPacketVar
{
	/// <summary>Item type ID (offset 0x04)</summary>
	public int typeID;

	/// <summary>Caster instance ID (offset 0x08)</summary>
	public long from;

	/// <summary>Hit tick (offset 0x10)</summary>
	public uint tick;

	// Variable length hit result data follows (not included in struct)
}
