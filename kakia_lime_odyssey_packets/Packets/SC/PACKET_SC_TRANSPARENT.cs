/// <summary>
/// Server packet changing the transparency/opacity of an object.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_TRANSPARENT
/// Size: 15 bytes
/// Ordinal: 2856
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX (2 bytes) - Packet header
/// - 0x02: __int64 objInstID (8 bytes) - Instance ID of object
/// - 0x0A: float value (4 bytes) - Transparency value (0.0 = fully transparent, 1.0 = fully opaque)
/// - 0x0E: bool smooth (1 byte) - Whether to smoothly interpolate to new transparency
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_TRANSPARENT
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the object</summary>
	public long objInstID;

	/// <summary>Transparency value (0.0 = fully transparent, 1.0 = fully opaque)</summary>
	public float value;

	/// <summary>Whether to smoothly interpolate to new transparency</summary>
	[MarshalAs(UnmanagedType.U1)]
	public bool smooth;
}
