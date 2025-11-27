using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// CS_NOTICE - Client requests system notice (query/acknowledgement)
///
/// IDA Verification Status: VERIFIED (2025-11-26)
/// IDA Structure Name: PACKET_CS_NOTICE
/// IDA Structure Size: 4 bytes (header only)
///
/// IDA Structure Layout:
/// +0x00: PACKET_VAR (header: ushort, size: ushort) - 4 bytes [handled by framework]
///
/// C# Implementation Notes:
/// - PACKET_VAR header (4 bytes) is stripped by RawPacket.ParsePackets
/// - This packet has no additional fields (header-only packet)
/// - Used to request or acknowledge system notices
/// - Server responds with SC_NOTICE
///
/// Type Mappings (IDA -> C#):
/// - No additional fields beyond header
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct CS_NOTICE
{
	// Header-only packet - no fields
}
