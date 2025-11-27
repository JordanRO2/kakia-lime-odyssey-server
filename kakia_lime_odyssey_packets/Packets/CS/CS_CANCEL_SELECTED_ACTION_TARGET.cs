using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// CS_CANCEL_SELECTED_ACTION_TARGET - Client cancels the currently selected action target
/// IDA Verification: PACKET_CS_CANCEL_SELECTED_ACTION_TARGET
/// Size: 2 bytes (2 byte header only)
/// Verified: 2025-11-26
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_CANCEL_SELECTED_ACTION_TARGET
{
}
