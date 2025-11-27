/// <summary>
/// Server->Client confirmation that looting has started.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_START_LOOTING
/// Size: 10 bytes (8 bytes + 2 byte PACKET_FIX header)
/// Fields:
///   - objInstID: Instance ID of the object being looted (8 bytes at offset 0x02)
/// Triggered by: CS_REQUEST_START_LOOTING
/// </remarks>
using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_START_LOOTING : IPacketFixed
{
	/// <summary>Instance ID of the object being looted</summary>
	public long objInstID;
}
