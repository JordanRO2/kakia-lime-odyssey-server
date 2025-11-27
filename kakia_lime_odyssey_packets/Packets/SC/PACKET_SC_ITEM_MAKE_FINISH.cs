/// <summary>
/// Server packet sent when crafting finishes (success or failure).
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_ITEM_MAKE_FINISH
/// Size: 19 bytes
/// Ordinal: 2628
/// Indicates crafting result and updates LP consumption.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_ITEM_MAKE_FINISH
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Result code (0=success, 1=normal craft, 2=critical, 3=failure)</summary>
	public byte finishResult;

	/// <summary>Instance ID of the crafter</summary>
	public long InstID;

	/// <summary>LP consumed for this craft</summary>
	public ushort useLP;

	/// <summary>Remaining LP after craft</summary>
	public ushort currentLP;

	/// <summary>Remaining count for continuous crafting</summary>
	public int remainCount;
}
