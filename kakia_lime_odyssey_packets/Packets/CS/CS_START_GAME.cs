using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client request to start the game with a selected character.
/// Sent after successful login to begin gameplay with a specific character slot.
/// </summary>
/// <remarks>
/// IDA Verified: Yes
/// IDA Struct: PACKET_CS_START_GAME
/// Size: 1 byte (3 bytes with PACKET_FIX header)
/// Verified Date: 2025-11-26
/// Response: SC_ENTER_ZONE, SC_ENTER_SIGHT_PC, SC_ENTER_SIGHT_NPC
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_START_GAME
{
	/// <summary>
	/// Character slot number (0-based index) to start the game with.
	/// Valid range: 0-2 (typically 3 character slots per account).
	/// </summary>
	public byte charNum;
}
