using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet to warp/teleport an object to a new position.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_WARP
/// Size: 34 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: __int64 objInstID (8 bytes)
/// - 0x0A: FPOS pos (12 bytes)
/// - 0x16: FPOS dir (12 bytes)
/// Triggered By: CS_ESCAPE, teleport events
/// Expected Response: CS_FINISH_WARP from client
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_WARP : IPacketFixed
{
	/// <summary>Object instance ID to warp (offset 0x02)</summary>
	public long objInstID;

	/// <summary>Target position coordinates x, y, z (offset 0x0A)</summary>
	public FPOS pos;

	/// <summary>Target direction vector x, y, z (offset 0x16)</summary>
	public FPOS dir;
}
