using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_KNIGHTS_SAY - Knights chat message broadcast
///
/// IDA Verification Status: VERIFIED (2025-11-26)
/// IDA Structure Name: PACKET_SC_KNIGHTS_SAY
/// IDA Structure Size: 20 bytes (base size, variable message)
///
/// IDA Structure Layout:
/// +0x00: PACKET_VAR (header: ushort, size: ushort) - 4 bytes [handled by framework]
/// +0x04: objInstID (__int64) - 8 bytes
/// +0x0C: maintainTime (unsigned int) - 4 bytes
/// +0x10: type (int) - 4 bytes
/// +0x14: message (variable length string)
///
/// C# Implementation Notes:
/// - PACKET_VAR header (4 bytes) is stripped by RawPacket.ParsePackets
/// - objInstID is the sender's object instance ID
/// - maintainTime specifies message display duration
/// - type contains message flags/type
/// - message is a variable-length ASCII string
/// - Broadcast to all knights faction members
///
/// Type Mappings (IDA -> C#):
/// - __int64 -> long (objInstID)
/// - unsigned int -> uint (maintainTime)
/// - int -> int (type)
/// - variable string -> string (message)
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct SC_KNIGHTS_SAY
{
	public long objInstID;
	public uint maintainTime;
	public int type;
	// Variable-length message text follows this header
}
