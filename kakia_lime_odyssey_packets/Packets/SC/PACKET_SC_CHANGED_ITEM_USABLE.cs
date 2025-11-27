/// <summary>
/// Server packet sent when item usability status changes.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_CHANGED_ITEM_USABLE
/// Size: 18 bytes
/// Ordinal: 18061
/// Controls whether items can be used (e.g., due to restrictions or buffs).
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_CHANGED_ITEM_USABLE : IPacketFixed
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the object</summary>
	public long objInstID;

	/// <summary>Current item usable value</summary>
	public int current;

	/// <summary>Item usable change amount (delta)</summary>
	public int update;

	public const int Size = 18;
}
