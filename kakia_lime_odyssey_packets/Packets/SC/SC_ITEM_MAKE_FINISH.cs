/// <summary>
/// Server->Client packet sent when crafting finishes (success, failure, or cancel).
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_ITEM_MAKE_FINISH
/// Size: 17 bytes (19 with PACKET_FIX header)
/// Triggered by: CS_ITEM_MAKE_CANCEL, CS_ITEM_MAKE_CONTINUALLY_STOP, craft completion
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_ITEM_MAKE_FINISH
{
	/// <summary>Result of the crafting attempt (0=fail, 1=success, 2=critical, 3=cancelled, etc.)</summary>
	public byte finishResult;

	/// <summary>Instance ID of the character crafting</summary>
	public long InstID;

	/// <summary>Life Points consumed by this craft</summary>
	public ushort useLP;

	/// <summary>Current Life Points remaining</summary>
	public ushort currentLP;

	/// <summary>Remaining count for continuous crafting (0 if done)</summary>
	public int remainCount;
}
