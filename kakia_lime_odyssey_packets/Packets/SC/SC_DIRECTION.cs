using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Notifies client of an entity's facing direction change.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_DIRECTION
/// Size: 26 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: __int64 objInstID (8 bytes) - Entity instance ID
/// - 0x0A: FPOS dir (12 bytes) - Direction vector
/// - 0x16: unsigned int tick (4 bytes) - Server tick timestamp
/// Triggered by: CS_DIRECTION_PC
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_DIRECTION : IPacketFixed
{
	/// <summary>Entity instance ID (offset 0x02)</summary>
	public long objInstID;

	/// <summary>Direction vector (offset 0x0A)</summary>
	public FPOS dir;

	/// <summary>Server tick timestamp (offset 0x16)</summary>
	public uint tick;
}
