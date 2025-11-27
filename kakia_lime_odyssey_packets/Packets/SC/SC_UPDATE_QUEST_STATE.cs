using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server notifies client about quest state/progress update.
/// Sent when quest objectives are completed or quest status changes.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_UPDATE_QUEST_STATE
/// Size: 9 bytes (IDA) = 4 bytes PACKET_VAR header + 5 bytes data
/// C# Size: 5 bytes (header stripped by framework)
/// Packet Type: PACKET_VAR (variable length header, 4 bytes, stripped by RawPacket.ParsePackets)
/// Memory Layout (IDA):
/// - 0x00: PACKET_VAR (base header) - 4 bytes [handled by framework]
/// - 0x04: unsigned int typeID - 4 bytes (quest type ID)
/// - 0x08: unsigned __int8 state - 1 byte (quest state)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_UPDATE_QUEST_STATE
{
	public uint typeID;
	public byte state;
}
