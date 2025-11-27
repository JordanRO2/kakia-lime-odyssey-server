using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client private message (whisper) from another player.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_WHISPER
/// Size: Variable (30+ bytes)
/// Memory Layout (IDA):
/// - 0x00: PACKET_VAR header (4 bytes) - handled by IPacketVar
/// - 0x04: char[26] fromName (26 bytes)
/// - 0x1E: variable-length message text
/// Note: Uses string properties for PacketWriter serialization convenience.
/// PacketWriter handles string-to-byte conversion internally.
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_WHISPER : IPacketVar
{
	/// <summary>Sender's character name (max 25 chars + null terminator)</summary>
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 26)]
	public string fromName;

	/// <summary>Whisper message content (variable length)</summary>
	public string message;
}
