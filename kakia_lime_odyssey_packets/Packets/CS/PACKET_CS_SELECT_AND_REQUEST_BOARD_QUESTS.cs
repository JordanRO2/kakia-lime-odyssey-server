/// <summary>
/// Client packet sent when player selects a quest board and requests available quests.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_SELECT_AND_REQUEST_BOARD_QUESTS
/// Size: 10 bytes
/// Ordinal: 2576
/// Combines quest board selection with quest list request.
/// Note: Field name is objInstID not targetInstID in IDA.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_SELECT_AND_REQUEST_BOARD_QUESTS
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the quest board object</summary>
	public long objInstID;
}
