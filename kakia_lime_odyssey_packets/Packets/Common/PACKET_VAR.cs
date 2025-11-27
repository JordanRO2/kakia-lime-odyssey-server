/// <summary>
/// Variable-length packet header structure containing opcode and size.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_VAR
/// Size: 4 bytes
/// This header is used for packets with variable-length data (arrays, strings, etc.).
/// The 'size' field indicates the total packet size including this header.
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Common;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_VAR
{
	/// <summary>Packet opcode/header identifying the packet type</summary>
	public ushort header;

	/// <summary>Total packet size in bytes (including this header)</summary>
	public ushort size;
}
