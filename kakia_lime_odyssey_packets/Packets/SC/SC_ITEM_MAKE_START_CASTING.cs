using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet to start crafting cast time.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_ITEM_MAKE_START_CASTING
/// Size: 19 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: __int64 InstID (8 bytes)
/// - 0x0A: unsigned int typeID (4 bytes)
/// - 0x0E: unsigned int castTime (4 bytes)
/// - 0x12: bool isCritical (1 byte)
/// Triggered by: CS_ITEM_MAKE_START, CS_ITEM_MAKE_CONTINUALLY
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_ITEM_MAKE_START_CASTING : IPacketFixed
{
	/// <summary>Instance ID of the character crafting (offset 0x02)</summary>
	public long InstID;

	/// <summary>Recipe type ID being crafted (offset 0x0A)</summary>
	public uint typeID;

	/// <summary>Cast time duration in milliseconds (offset 0x0E)</summary>
	public uint castTime;

	/// <summary>Whether this craft will result in critical success (offset 0x12)</summary>
	public bool isCritical;
}
