using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_TALKING - NPC dialog message
/// IDA Structure: PACKET_SC_TALKING (12 bytes)
/// Verified against IDA Pro: 2025-11-26
///
/// Server sends this to display NPC dialog text to the client.
/// Variable-length packet with dialog string appended after the base structure.
///
/// Structure layout:
/// 0x00: PACKET_VAR (4 bytes) - base packet header with variable data
/// 0x04: __int64 objInstID - NPC instance ID
/// Variable: dialog string data follows
/// Total: 12 bytes + string data
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct SC_TALKING : IPacketVar
{
	public long objInstID;
	public string dialog;
}

/// <summary>
/// SC_TALKING_CHOICE - NPC dialog choice menu
/// IDA Structure: PACKET_SC_TALKING_CHOICE (12 bytes)
/// Verified against IDA Pro: 2025-11-26
///
/// Server sends this to display choice menu in NPC dialog.
/// Variable-length packet with choice strings appended after the base structure.
///
/// Structure layout:
/// 0x00: PACKET_VAR (4 bytes) - base packet header with variable data
/// 0x04: __int64 objInstID - NPC instance ID
/// Variable: choice menu data follows
/// Total: 12 bytes + choice data
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct SC_TALKING_CHOICE : IPacketVar
{
	public long objInstID;
}