/// <summary>
/// Base packet header structure containing the packet opcode.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_FIX
/// Size: 2 bytes
/// This is the base header present in all packets.
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Common;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_FIX
{
	/// <summary>Packet opcode/header identifying the packet type</summary>
	public ushort header;
}
