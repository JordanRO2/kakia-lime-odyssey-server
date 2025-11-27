using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client notification of continuous action start (gathering, crafting, etc).
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_START_CONTINUOUS_ACTION
/// Size: 14 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: __int64 instID (8 bytes)
/// - 0x0A: unsigned int action (4 bytes)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_START_CONTINUOUS_ACTION : IPacketFixed
{
	/// <summary>Object instance ID performing the action (offset 0x02)</summary>
	public long instID;

	/// <summary>Action type ID (offset 0x0A)</summary>
	public uint action;
}
