using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Notifies client to release/reset combo state
/// Verified against IDA: PACKET_SC_RELEASE_COMBO (Size: 2 bytes)
/// Date: 2025-11-26
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_RELEASE_COMBO : IPacketFixed
{
	// This packet only contains the header (PACKET_FIX), no additional fields
}
