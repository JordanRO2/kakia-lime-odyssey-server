/// <summary>
/// Server packet sent when item casting adjustment changes.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_CHANGED_ADJUST_ITEM_CASTING
/// Size: 18 bytes
/// Ordinal: 22226
/// Affects the casting time for items (separate from casting ratio).
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_CHANGED_ADJUST_ITEM_CASTING : IPacketFixed
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the object</summary>
	public long objInstID;

	/// <summary>Current item casting adjustment value</summary>
	public int current;

	/// <summary>Item casting adjustment change amount (delta)</summary>
	public int update;

	public const int Size = 18;
}
