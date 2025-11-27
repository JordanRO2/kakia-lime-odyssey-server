using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// CS_SELECT_AND_REQUEST_TALKING - Select NPC and request dialog
/// IDA Structure: PACKET_CS_SELECT_AND_REQUEST_TALKING (10 bytes)
/// Verified against IDA Pro: 2025-11-26
///
/// Client sends this when player selects an NPC and initiates conversation.
///
/// Structure layout:
/// 0x00: PACKET_FIX (2 bytes) - base packet header
/// 0x02: __int64 targetInstID - NPC instance ID
/// Total: 10 bytes
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_SELECT_AND_REQUEST_TALKING
{
	public long targetInstID;
}
