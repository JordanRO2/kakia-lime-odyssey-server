/// <summary>
/// Server packet changing the RGB color tint of an object.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_COLOR
/// Size: 13 bytes
/// Ordinal: 2857
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX (2 bytes) - Packet header
/// - 0x02: __int64 instID (8 bytes) - Instance ID of object to tint
/// - 0x0A: COLOR color (3 bytes) - RGB color values
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_COLOR
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the object to tint</summary>
	public long instID;

	/// <summary>RGB color tint to apply</summary>
	public COLOR color;
}
