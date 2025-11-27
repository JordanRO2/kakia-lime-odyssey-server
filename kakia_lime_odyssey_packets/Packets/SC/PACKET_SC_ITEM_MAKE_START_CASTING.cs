/// <summary>
/// Server packet sent when crafting (item making) begins casting.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_ITEM_MAKE_START_CASTING
/// Size: 19 bytes
/// Ordinal: 2627
/// Initiates crafting cast bar and animation.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_ITEM_MAKE_START_CASTING
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the crafter</summary>
	public long InstID;

	/// <summary>Item type ID being crafted</summary>
	public uint typeID;

	/// <summary>Total crafting time in milliseconds</summary>
	public uint castTime;

	/// <summary>Whether this is a critical craft (bonus quality/quantity)</summary>
	public bool isCritical;
}
