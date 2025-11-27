/// <summary>
/// Server->Client packet to start crafting cast time.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_ITEM_MAKE_START_CASTING
/// Size: 17 bytes (19 with PACKET_FIX header)
/// Triggered by: CS_ITEM_MAKE_START, CS_ITEM_MAKE_CONTINUALLY
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_ITEM_MAKE_START_CASTING
{
	/// <summary>Instance ID of the character crafting</summary>
	public long InstID;

	/// <summary>Recipe type ID being crafted</summary>
	public uint typeID;

	/// <summary>Cast time duration in milliseconds</summary>
	public uint castTime;

	/// <summary>Whether this craft will result in critical success</summary>
	public bool isCritical;
}
