using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet sent when crafting finishes (success, failure, or cancel).
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_ITEM_MAKE_FINISH
/// Size: 19 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: unsigned __int8 finishResult (1 byte)
/// - 0x03: __int64 InstID (8 bytes)
/// - 0x0B: unsigned __int16 useLP (2 bytes)
/// - 0x0D: unsigned __int16 currentLP (2 bytes)
/// - 0x0F: int remainCount (4 bytes)
/// Triggered by: CS_ITEM_MAKE_CANCEL, CS_ITEM_MAKE_CONTINUALLY_STOP, craft completion
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_ITEM_MAKE_FINISH : IPacketFixed
{
	/// <summary>Result of the crafting attempt (0=fail, 1=success, 2=critical, 3=cancelled) (offset 0x02)</summary>
	public byte finishResult;

	/// <summary>Instance ID of the character crafting (offset 0x03)</summary>
	public long InstID;

	/// <summary>Life Points consumed by this craft (offset 0x0B)</summary>
	public ushort useLP;

	/// <summary>Current Life Points remaining (offset 0x0D)</summary>
	public ushort currentLP;

	/// <summary>Remaining count for continuous crafting (0 if done) (offset 0x0F)</summary>
	public int remainCount;
}
