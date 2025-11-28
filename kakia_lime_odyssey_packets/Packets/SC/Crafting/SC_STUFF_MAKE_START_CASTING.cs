/// <summary>
/// SC_ version of stuff make (gathering) start casting packet without PACKET_FIX header.
/// </summary>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Interface;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_STUFF_MAKE_START_CASTING (18 bytes total)
/// Initiates gathering cast bar and animation.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_STUFF_MAKE_START_CASTING : IPacketFixed
{
	/// <summary>Instance ID of the gatherer</summary>
	public long InstID;

	/// <summary>Resource type ID being gathered</summary>
	public uint typeID;

	/// <summary>Total gathering time in milliseconds</summary>
	public uint castTime;
}
