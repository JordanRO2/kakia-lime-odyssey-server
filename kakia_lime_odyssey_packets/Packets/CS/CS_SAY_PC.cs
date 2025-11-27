using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// CS_SAY_PC - Local chat message packet
///
/// IDA Verification Status: VERIFIED (2025-11-26)
/// IDA Structure Name: PACKET_CS_SAY_PC
/// IDA Structure Size: 12 bytes (including PACKET_VAR header)
///
/// IDA Structure Layout:
/// +0x00: PACKET_VAR (header: ushort, size: ushort) - 4 bytes [handled by framework]
/// +0x04: maintainTime (unsigned int) - 4 bytes
/// +0x08: type (int) - 4 bytes
/// +0x0C: message (variable length string)
///
/// C# Implementation Notes:
/// - PACKET_VAR header (4 bytes) is stripped by RawPacket.ParsePackets
/// - PacketReader.Read_CS_SAY_PC skips the size field (2 bytes)
/// - maintainTime and type are read as fixed fields
/// - message is read as variable-length ASCII string (packet_size - 8 bytes)
///
/// Type Mappings (IDA -> C#):
/// - unsigned int -> uint (maintainTime)
/// - int -> int (type)
/// - variable string -> string (message)
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct CS_SAY_PC
{
	public uint maintainTime;
	public int type;
	public string message;
}
