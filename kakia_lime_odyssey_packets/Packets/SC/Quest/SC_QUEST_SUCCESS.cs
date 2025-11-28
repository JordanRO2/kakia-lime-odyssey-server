using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Notifies client that quest objectives are complete (ready to turn in).
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_QUEST_SUCCESS
/// Size: 6 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: unsigned int typeID (4 bytes) - Quest type identifier
/// Triggered by: All quest objectives completed
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_QUEST_SUCCESS : IPacketFixed
{
	/// <summary>Quest type identifier (offset 0x02)</summary>
	public uint typeID;
}
