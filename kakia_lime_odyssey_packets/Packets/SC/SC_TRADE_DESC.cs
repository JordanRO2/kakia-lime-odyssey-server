using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet describing NPC merchant trade window details.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_TRADE_DESC
/// Size: 21 bytes total (variable-length packet)
/// Memory Layout (IDA):
/// - 0x00: PACKET_VAR header (4 bytes) - handled by IPacketVar
/// - 0x04: __int64 objInstID (8 bytes)
/// - 0x0C: unsigned int itemCount (4 bytes)
/// - 0x10: int discountRate (4 bytes)
/// - 0x14: bool isRepairable (1 byte)
/// Triggered by: CS_REQUEST_TRADE, CS_SELECT_TARGET_REQUEST_TRADE
/// Note: Variable-length packet - contains array of items merchant sells
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_TRADE_DESC : IPacketVar
{
	/// <summary>Object instance ID of the NPC merchant (offset 0x04)</summary>
	public long objInstID;

	/// <summary>Number of items in merchant's inventory (offset 0x0C)</summary>
	public uint itemCount;

	/// <summary>Discount rate percentage applied to prices (offset 0x10)</summary>
	public int discountRate;

	/// <summary>Whether this merchant can repair items (offset 0x14)</summary>
	public bool isRepairable;

	// Note: Followed by itemCount array of item data (handled separately)
}
