using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server private message (whisper) packet.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_WHISPER_PC
/// Size: Variable (30+ bytes with PACKET_VAR header)
/// Memory Layout (IDA):
/// - 0x00: PACKET_VAR header (4 bytes) - handled by IPacketVar
/// - 0x04: char[26] targetPCName (26 bytes)
/// - 0x1E: variable-length message text
/// Response: SC_WHISPER (sent to target player)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_WHISPER_PC : IPacketVar
{
	/// <summary>Target player character name (offset 0x04)</summary>
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 26)]
	public string targetPCName;

	/// <summary>Whisper message content</summary>
	public string message;
}
