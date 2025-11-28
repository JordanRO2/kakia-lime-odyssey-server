/// <summary>
/// Client packet sent when player selects an NPC and requests to start talking.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_SELECT_AND_REQUEST_TALKING
/// Size: 10 bytes
/// Ordinal: 2568
/// Combines target selection with dialog initiation request.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_SELECT_AND_REQUEST_TALKING
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the target NPC</summary>
	public long targetInstID;
}
