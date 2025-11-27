using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_PC_STATUS - Server to Client player character status packet
/// Total size: 186 bytes (including 2-byte PACKET_FIX header)
/// IDA verified: 2025-11-26
/// Structure name in client: PACKET_SC_PC_STATUS
/// </summary>
/// <remarks>
/// This packet is sent to update the client with complete player character status information.
/// Contains all combat stats, job information, velocities, and other character attributes.
/// Triggered by: CS_REQUEST_PC_STATUS or automatic status updates
/// IDA Details:
/// - Total size: 186 bytes
/// - Member count: 3 (PACKET_FIX header + objInstID + status)
/// - Offset 0x02: __int64 objInstID (8 bytes)
/// - Offset 0x0A: STATUS_PC status (176 bytes)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_PC_STATUS
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
