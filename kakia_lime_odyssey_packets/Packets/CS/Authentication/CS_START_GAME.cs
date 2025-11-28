using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client request to start the game with a selected character.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_START_GAME
/// Size: 3 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: unsigned char charNum (1 byte)
/// Response: SC_ENTER_ZONE, SC_ENTER_SIGHT_PC, SC_ENTER_SIGHT_NPC
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_START_GAME : IPacketFixed
{
	/// <summary>
	/// Character slot number (0-based index) to start the game with.
	/// Valid range: 0-2 (typically 3 character slots per account).
	/// </summary>
	public byte charNum;
}
