/// <summary>
/// Client packet sent when player selects a lootable object and requests to start looting.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_SELECT_TARGET_REQUEST_START_LOOTING
/// Size: 10 bytes
/// Ordinal: 2701
/// Combines target selection with loot initiation request.
/// Note: Field name is objInstID not targetInstID in IDA.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_SELECT_TARGET_REQUEST_START_LOOTING
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the lootable object</summary>
	public long objInstID;
}
