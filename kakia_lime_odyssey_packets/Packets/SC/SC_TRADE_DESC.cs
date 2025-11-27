/// <summary>
/// Server->Client packet describing NPC merchant trade window details.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_TRADE_DESC
/// Size: 17 bytes (21 with PACKET_VAR header)
/// Triggered by: CS_REQUEST_TRADE, CS_SELECT_TARGET_REQUEST_TRADE
/// Note: Variable-length packet - contains array of items merchant sells
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_TRADE_DESC
{
	/// <summary>Object instance ID of the NPC merchant</summary>
	public long objInstID;

	/// <summary>Number of items in merchant's inventory</summary>
	public uint itemCount;

	/// <summary>Discount rate percentage applied to prices</summary>
	public int discountRate;

	/// <summary>Whether this merchant can repair items</summary>
	public bool isRepairable;

	// Note: Followed by itemCount array of item data (handled separately)
}
