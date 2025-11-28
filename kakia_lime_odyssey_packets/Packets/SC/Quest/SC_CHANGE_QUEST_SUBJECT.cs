using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet indicating a quest subject/objective has changed.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_CHANGE_QUEST_SUBJECT
/// Size: 8 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: unsigned int typeID (4 bytes)
/// - 0x06: unsigned __int8 subjectNum (1 byte)
/// - 0x07: bool isSuccessed (1 byte)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_CHANGE_QUEST_SUBJECT : IPacketFixed
{
	/// <summary>Quest type ID (offset 0x02)</summary>
	public uint typeID;

	/// <summary>Subject/objective number (offset 0x06)</summary>
	public byte subjectNum;

	/// <summary>Whether the subject was completed successfully (offset 0x07)</summary>
	public bool isSuccessed;
}
