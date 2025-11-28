using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Notifies client that an entity has stood up.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_STAND_UP
/// Size: 14 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: __int64 objInstID (8 bytes) - Entity instance ID
/// - 0x0A: unsigned int tick (4 bytes) - Server tick timestamp
/// Triggered by: CS_STAND_UP_PC
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_STAND_UP : IPacketFixed
{
	/// <summary>Entity instance ID (offset 0x02)</summary>
	public long objInstID;

	/// <summary>Server tick timestamp (offset 0x0A)</summary>
	public uint tick;
}