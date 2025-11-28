using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet notifying client to enter the lobby with encryption keys.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_ENTER_LOBBY
/// Size: 10 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: int key1 (4 bytes)
/// - 0x06: int key2 (4 bytes)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_ENTER_LOBBY : IPacketFixed
{
	/// <summary>First encryption key (offset 0x02)</summary>
	public int key1;

	/// <summary>Second encryption key (offset 0x06)</summary>
	public int key2;
}
