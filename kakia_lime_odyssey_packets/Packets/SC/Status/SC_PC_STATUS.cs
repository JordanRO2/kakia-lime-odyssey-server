using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet containing full player character status.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_PC_STATUS
/// Size: 186 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: __int64 objInstID (8 bytes)
/// - 0x0A: STATUS_PC status (176 bytes)
/// Triggered by: CS_REQUEST_PC_STATUS, status changes
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_PC_STATUS : IPacketFixed
{
	/// <summary>
	/// Object instance ID of the player character
	/// Offset: 0x02 (after PACKET_FIX header)
	/// Type: __int64 (long)
	/// </summary>
	public long objInstID;

	/// <summary>
	/// Complete player character status structure
	/// Offset: 0x0A
	/// Type: STATUS_PC (176 bytes)
	/// Contains all combat stats, job info, velocities, and character attributes
	/// </summary>
	public STATUS_PC status;
}
