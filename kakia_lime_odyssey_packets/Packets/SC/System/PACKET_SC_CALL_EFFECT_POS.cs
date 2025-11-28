/// <summary>
/// Server packet triggering a visual effect at a specific world position.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_CALL_EFFECT_POS
/// Size: 18 bytes
/// Ordinal: 2859
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX (2 bytes) - Packet header
/// - 0x02: POS pos (12 bytes) - World position (x, y, z coordinates)
/// - 0x0E: int effectID (4 bytes) - Effect resource ID
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_CALL_EFFECT_POS
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>World position where the effect should be played</summary>
	public POS pos;

	/// <summary>Effect resource ID to play</summary>
	public int effectID;
}
