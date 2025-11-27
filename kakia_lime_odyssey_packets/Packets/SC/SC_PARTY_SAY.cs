using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client party chat message.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_PARTY_SAY
/// Size: 16 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_VAR header (4 bytes) - handled by IPacketVar
/// - 0x04: unsigned int idx (4 bytes)
/// - 0x08: unsigned int maintainTime (4 bytes)
/// - 0x0C: int type (4 bytes)
/// Note: Variable-length message text follows this struct
/// Triggered by: CS_PARTY_SAY_PC
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_PARTY_SAY : IPacketVar
{
	/// <summary>Party member index who sent message (offset 0x04)</summary>
	public uint idx;

	/// <summary>Message maintain time in milliseconds (offset 0x08)</summary>
	public uint maintainTime;

	/// <summary>Chat type/flags (offset 0x0C)</summary>
	public int type;

	// Note: Variable-length message text follows this struct
}
