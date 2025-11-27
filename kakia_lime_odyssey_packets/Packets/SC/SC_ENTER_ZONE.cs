using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server notifies client to enter a zone with specified object instance ID.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_ENTER_ZONE
/// Size: 10 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (ushort header) - 2 bytes (handled by IPacketFixed)
/// - 0x02: __int64 objInstID - 8 bytes
/// Triggered by: CS_START_GAME or zone transition requests
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_ENTER_ZONE : IPacketFixed
{
	public long objInstID;
}
