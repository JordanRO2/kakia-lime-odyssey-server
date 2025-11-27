using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet indicating crafting material addition was successful.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_STUFF_MAKE_ADD_LIST_SUCCESS
/// Size: 112 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: unsigned int typeID (4 bytes)
/// - 0x06: int successPercent (4 bytes)
/// - 0x0A: unsigned int makeTime (4 bytes)
/// - 0x0E: unsigned __int16 requestLP (2 bytes)
/// - 0x10: STUFF_MAKE_ITEMS resultItems (80 bytes)
/// - 0x60: STUFF_MAKE_SLOT addedItem (16 bytes)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_STUFF_MAKE_ADD_LIST_SUCCESS : IPacketFixed
{
	/// <summary>Crafting recipe type ID (offset 0x02)</summary>
	public uint typeID;

	/// <summary>Success percentage (offset 0x06)</summary>
	public int successPercent;

	/// <summary>Time to complete crafting (offset 0x0A)</summary>
	public uint makeTime;

	/// <summary>LP required (offset 0x0E)</summary>
	public ushort requestLP;

	/// <summary>Result items from crafting (offset 0x10)</summary>
	public STUFF_MAKE_ITEMS resultItems;

	/// <summary>Item that was added (offset 0x60)</summary>
	public STUFF_MAKE_SLOT addedItem;
}
