/// <summary>
/// Client->Server packet to cancel weapon ready state.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_CANCEL_READY_WEAPON_HITING
/// Size: 2 bytes (header-only)
/// Triggered by: Client canceling weapon ready/charging state
/// Response: SC_CANCEL_READY_WEAPON_HITING
/// </remarks>
using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_CANCEL_READY_WEAPON_HITING : IPacketFixed
{
    // Header-only packet - no additional fields
}
