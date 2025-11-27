using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet to apply a color tint to an object.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_COLOR
/// Size: 13 bytes
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes)
/// - 0x02: __int64 instID (8 bytes) - Object instance ID
/// - 0x0A: COLOR color (3 bytes) - RGB color tint
/// Used to apply color overlays/tints to objects for visual effects or status indicators.
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_COLOR : IPacketFixed
{
	public long instID;
	public COLOR color;
}
