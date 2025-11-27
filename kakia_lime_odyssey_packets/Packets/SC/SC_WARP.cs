/// <summary>
/// Server->Client packet to warp/teleport an object to a new position.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_WARP
/// Size: 32 bytes (34 with PACKET_FIX header)
/// Triggered By: CS_ESCAPE, teleport events
/// Fields:
/// - objInstID: Object instance ID to warp (8 bytes, __int64)
/// - pos: Target position (12 bytes, FPOS struct with 3 floats)
/// - dir: Target direction (12 bytes, FPOS struct with 3 floats)
/// Expected Response: CS_FINISH_WARP from client
/// </remarks>
using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_WARP : IPacketFixed
{
	/// <summary>Object instance ID to warp</summary>
	public long objInstID;

	/// <summary>Target position coordinates (x, y, z)</summary>
	public FPOS pos;

	/// <summary>Target direction vector (x, y, z)</summary>
	public FPOS dir;
}
