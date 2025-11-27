using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// CS_REALM_SAY_PC - Realm-wide chat message packet
///
/// IDA Verification Status: VERIFIED (2025-11-26)
/// IDA Structure Name: PACKET_CS_REALM_SAY_PC
/// IDA Structure Size: 12 bytes (base size, variable message)
///
/// IDA Structure Layout:
/// +0x00: PACKET_VAR (header: ushort, size: ushort) - 4 bytes [handled by framework]
/// +0x04: maintainTime (unsigned int) - 4 bytes
/// +0x08: type (int) - 4 bytes
/// +0x0C: message (variable length string)
///
/// C# Implementation Notes:
/// - PACKET_VAR header (4 bytes) is stripped by RawPacket.ParsePackets
/// - maintainTime and type are read as fixed fields
/// - message is read as variable-length ASCII string
/// - Realm chat is broadcast to all players in the realm/faction
///
/// Type Mappings (IDA -> C#):
/// - unsigned int -> uint (maintainTime)
/// - int -> int (type)
/// - variable string -> string (message)
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct CS_REALM_SAY_PC
{
	public uint maintainTime;
	public int type;
	// Variable-length message text follows this header
}
