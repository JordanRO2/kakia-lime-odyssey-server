/// <summary>
/// Server packet to trigger a camera macro/cutscene.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_MACRO_CAMERA
/// Size: 6 bytes
/// Ordinal: 2866
/// Triggers cinematic camera movements or cutscenes by index.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_MACRO_CAMERA
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Camera macro index to execute</summary>
	public uint idx;
}
