/// <summary>
/// Client packet requesting to prepare for stuff/material making (gathering).
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_STUFF_MAKE_READY
/// Size: 6 bytes
/// Ordinal: 2632
/// Sent when opening the gathering/material processing interface.
/// Server responds with SC_STUFF_MAKE_READY_SUCCESS.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_STUFF_MAKE_READY
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Type ID of the gathering/processing action</summary>
	public uint typeID;
}
