using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_SHOUT - Zone-wide shout message broadcast
///
/// IDA Verification Status: VERIFIED (2025-11-26)
/// IDA Structure Name: PACKET_SC_SHOUT
/// IDA Structure Size: 38 bytes (base size, variable message)
///
/// IDA Structure Layout:
/// +0x00: PACKET_VAR (header: ushort, size: ushort) - 4 bytes [handled by framework]
/// +0x04: instID (__int64) - 8 bytes
/// +0x0C: name (char[26]) - 26 bytes
/// +0x26: message (variable length string)
///
/// C# Implementation Notes:
/// - PACKET_VAR header (4 bytes) is stripped by RawPacket.ParsePackets
/// - instID is the shouter's object instance ID
/// - name is a fixed 26-byte character array for the shouter's name
/// - message is a variable-length ASCII string
/// - Broadcast to entire zone/map
///
/// Type Mappings (IDA -> C#):
/// - __int64 -> long (instID)
/// - char[26] -> string (name, max 25 chars + null terminator)
/// - variable string -> string (message)
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct SC_SHOUT
{
	public long instID;

	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 26)]
	public string name;

	public string message;
}
