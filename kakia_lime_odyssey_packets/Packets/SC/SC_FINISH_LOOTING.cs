/// <summary>
/// Server->Client notification that looting has finished.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_FINISH_LOOTING
/// Size: 10 bytes (8 bytes + 2 byte PACKET_FIX header)
/// Fields:
///   - objInstID: Instance ID of the object that was being looted (8 bytes at offset 0x02)
/// Triggered by: CS_FINISH_LOOTING or loot window closed
/// </remarks>
using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_FINISH_LOOTING : IPacketFixed
{
	/// <summary>Instance ID of the object that was being looted</summary>
	public long objInstID;
}
