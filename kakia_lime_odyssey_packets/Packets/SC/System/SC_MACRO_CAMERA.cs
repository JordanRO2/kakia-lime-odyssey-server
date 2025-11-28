using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Interface;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet to trigger a camera macro/preset.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_MACRO_CAMERA
/// Size: 6 bytes
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes)
/// - 0x02: unsigned int idx (4 bytes) - Camera macro index/ID
/// Used for cinematic camera movements, cutscenes, or predefined camera angles.
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_MACRO_CAMERA : IPacketFixed
{
	public uint idx;
}
