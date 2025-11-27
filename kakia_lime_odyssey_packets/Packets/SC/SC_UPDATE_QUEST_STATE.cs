using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Notifies client of a quest state/progress update.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_UPDATE_QUEST_STATE
/// Size: 9 bytes total (variable-length packet)
/// Memory Layout (IDA):
/// - 0x00: PACKET_VAR header (4 bytes) - handled by IPacketVar
/// - 0x04: unsigned int typeID (4 bytes) - Quest type identifier
/// - 0x08: unsigned __int8 state (1 byte) - New quest state
/// Triggered by: Quest objective completion, quest progress
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_UPDATE_QUEST_STATE : IPacketVar
{
	/// <summary>Quest type identifier (offset 0x04)</summary>
	public uint typeID;

	/// <summary>New quest state (offset 0x08)</summary>
	public byte state;
}
