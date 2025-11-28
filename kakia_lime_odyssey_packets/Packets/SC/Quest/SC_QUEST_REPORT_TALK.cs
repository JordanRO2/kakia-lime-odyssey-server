using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet for quest report dialog.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_QUEST_REPORT_TALK
/// Size: 16 bytes total (variable-length packet)
/// Memory Layout (IDA):
/// - 0x00: PACKET_VAR header (4 bytes) - handled by IPacketVar
/// - 0x04: unsigned int typeID (4 bytes)
/// - 0x08: __int64 objInstID (8 bytes)
/// Note: Variable-length - dialog data follows the fixed fields
/// Triggered by: Quest turn-in NPC interaction
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_QUEST_REPORT_TALK : IPacketVar
{
	/// <summary>Quest type ID (offset 0x04)</summary>
	public uint typeID;

	/// <summary>NPC instance ID (offset 0x08)</summary>
	public long objInstID;

	// Variable length dialog data follows (not included in struct)
}
