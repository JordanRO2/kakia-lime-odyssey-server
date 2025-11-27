using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_WHISPER - Private message (whisper) from another player
///
/// IDA Verification Status: VERIFIED (2025-11-26)
/// IDA Structure Name: PACKET_SC_WHISPER
/// IDA Structure Size: 30 bytes (including PACKET_VAR header)
///
/// IDA Structure Layout:
/// +0x00: PACKET_VAR (header: ushort, size: ushort) - 4 bytes [handled by framework]
/// +0x04: fromName (char[26]) - 26 bytes
///
/// C# Implementation Notes:
/// - PACKET_VAR header (4 bytes) is stripped by RawPacket.ParsePackets
/// - fromName is a fixed 26-byte character array for the sender's name
/// - Message content is passed as variable-length string after fixed fields
///
/// Type Mappings (IDA -> C#):
/// - char[26] -> string (fromName, max 25 chars + null terminator)
/// - variable string -> string (message)
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct SC_WHISPER
{
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 26)]
	public string fromName;
	public string message;
}
