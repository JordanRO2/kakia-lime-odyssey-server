using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_NOTICE - System notice/announcement message
///
/// IDA Verification Status: VERIFIED (2025-11-26)
/// IDA Structure Name: PACKET_SC_NOTICE
/// IDA Structure Size: 30 bytes (base size, variable message)
///
/// IDA Structure Layout:
/// +0x00: PACKET_VAR (header: ushort, size: ushort) - 4 bytes [handled by framework]
/// +0x04: from (char[26]) - 26 bytes
/// +0x1E: message (variable length string)
///
/// C# Implementation Notes:
/// - PACKET_VAR header (4 bytes) is stripped by RawPacket.ParsePackets
/// - from is a fixed 26-byte character array for the source/sender name
/// - message is a variable-length ASCII string
/// - Used for system announcements and important notices
///
/// Type Mappings (IDA -> C#):
/// - char[26] -> string (from, max 25 chars + null terminator)
/// - variable string -> string (message)
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct SC_NOTICE
{
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 26)]
	public string from;

	public string message;
}
