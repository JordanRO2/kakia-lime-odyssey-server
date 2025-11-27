using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet for quest failed.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_QUEST_FAIL
/// Size: 6 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: unsigned int typeID (4 bytes)
/// Triggered by: Quest failure conditions (time limit, failed objective)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_QUEST_FAIL : IPacketFixed
{
	/// <summary>Quest type ID (offset 0x02)</summary>
	public uint typeID;
}
