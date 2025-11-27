using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client base packet when zone objects enter player's view range.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_ENTER_ZONEOBJ
/// Size: 36 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_VAR header (4 bytes) - handled by IPacketVar
/// - 0x04: __int64 objInstID (8 bytes)
/// - 0x0C: FPOS pos (12 bytes)
/// - 0x18: FPOS dir (12 bytes)
/// Note: This is a base packet extended by SC_ENTER_SIGHT_* packets
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_ENTER_ZONEOBJ : IPacketVar
{
	/// <summary>Object instance ID (offset 0x04)</summary>
	public long objInstID;

	/// <summary>Object position (offset 0x0C)</summary>
	public FPOS pos;

	/// <summary>Object direction (offset 0x18)</summary>
	public FPOS dir;
}
