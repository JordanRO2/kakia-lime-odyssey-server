/// <summary>
/// Server packet sent when gathering (stuff making) begins casting.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_STUFF_MAKE_START_CASTING
/// Size: 18 bytes
/// Ordinal: 2641
/// Initiates gathering cast bar and animation.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_STUFF_MAKE_START_CASTING
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the gatherer</summary>
	public long InstID;

	/// <summary>Resource type ID being gathered</summary>
	public uint typeID;

	/// <summary>Total gathering time in milliseconds</summary>
	public uint castTime;
}
