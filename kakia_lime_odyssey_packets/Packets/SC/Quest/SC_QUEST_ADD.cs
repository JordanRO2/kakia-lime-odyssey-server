using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Notifies client that a quest was added to their quest log.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_QUEST_ADD
/// Size: 8 bytes total (variable-length packet)
/// Memory Layout (IDA):
/// - 0x00: PACKET_VAR header (4 bytes) - handled by IPacketVar
/// - 0x04: unsigned int typeID (4 bytes) - Quest type identifier
/// Triggered by: CS_QUEST_ADD, NPC dialog quest accept
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_QUEST_ADD : IPacketVar
{
	/// <summary>Quest type identifier (offset 0x04)</summary>
	public uint typeID;
}
