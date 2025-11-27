/// <summary>
/// Server packet triggering a sound effect attached to an object.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_CALL_SOUND
/// Size: 14 bytes
/// Ordinal: 2860
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX (2 bytes) - Packet header
/// - 0x02: __int64 objInstID (8 bytes) - Instance ID of object to attach sound to
/// - 0x0A: int soundID (4 bytes) - Sound resource ID
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_CALL_SOUND
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the object to attach the sound to</summary>
	public long objInstID;

	/// <summary>Sound resource ID to play</summary>
	public int soundID;
}
