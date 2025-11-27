using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

/// <summary>
/// RGB color structure used for NPC/object tinting.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: COLOR
/// Size: 3 bytes
/// Memory Layout (IDA):
/// - 0x00: unsigned __int8 r - 1 byte (red channel)
/// - 0x01: unsigned __int8 g - 1 byte (green channel)
/// - 0x02: unsigned __int8 b - 1 byte (blue channel)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct COLOR
{
	public byte r;
	public byte g;
	public byte b;
}
