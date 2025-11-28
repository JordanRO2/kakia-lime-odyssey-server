/// <summary>
/// Server packet changing the visual scale/size of an object.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_CHANGE_SCALE
/// Size: 15 bytes
/// Ordinal: 2855
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX (2 bytes) - Packet header
/// - 0x02: __int64 objInstID (8 bytes) - Instance ID of object to scale
/// - 0x0A: float scale (4 bytes) - Scale multiplier (1.0 = normal size)
/// - 0x0E: bool smooth (1 byte) - Whether to smoothly interpolate to new scale
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_CHANGE_SCALE
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the object to scale</summary>
	public long objInstID;

	/// <summary>Scale multiplier (1.0 = normal size, 2.0 = double size, etc.)</summary>
	public float scale;

	/// <summary>Whether to smoothly interpolate to new scale</summary>
	[MarshalAs(UnmanagedType.U1)]
	public bool smooth;
}
