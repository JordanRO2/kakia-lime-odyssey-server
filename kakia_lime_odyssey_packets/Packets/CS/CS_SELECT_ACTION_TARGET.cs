using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// CS_SELECT_ACTION_TARGET - Client selects a target for action
/// IDA Verification: PACKET_CS_SELECT_ACTION_TARGET
/// Size: 10 bytes (2 byte header + 8 byte targetInstID)
/// Verified: 2025-11-26
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_SELECT_ACTION_TARGET
{
	public long targetInstID;
}
