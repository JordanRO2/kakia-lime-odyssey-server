/// <summary>
/// Server packet sent when an entity's movement velocities change.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_CHANGED_VELOCITIES
/// Size: 46 bytes
/// Ordinal: 2617
/// Contains all movement speeds (run, walk, swim, etc.) and their acceleration values.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_CHANGED_VELOCITIES
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>New velocity values for all movement types</summary>
	public VELOCITIES velocities;
}
