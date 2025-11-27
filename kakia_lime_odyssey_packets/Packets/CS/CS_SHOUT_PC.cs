using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// CS_SHOUT_PC - Zone-wide shout message packet
///
/// IDA Verification Status: VERIFIED (2025-11-26)
/// IDA Structure Name: PACKET_CS_SHOUT_PC
/// IDA Structure Size: 4 bytes (base size, variable message)
///
/// IDA Structure Layout:
/// +0x00: PACKET_VAR (header: ushort, size: ushort) - 4 bytes [handled by framework]
/// +0x04: message (variable length string)
///
/// C# Implementation Notes:
/// - PACKET_VAR header (4 bytes) is stripped by RawPacket.ParsePackets
/// - message is a variable-length ASCII string
/// - Shout is broadcast to entire zone/map
///
/// Type Mappings (IDA -> C#):
/// - variable string -> string (message)
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct CS_SHOUT_PC
{
	public string message;
}
