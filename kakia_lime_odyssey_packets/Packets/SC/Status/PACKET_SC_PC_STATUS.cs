/// <summary>
/// Server packet containing full player character status.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_PC_STATUS
/// Size: 186 bytes
/// Ordinal: 2600
/// Contains complete status information including combat stats, job info, and velocities.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_PC_STATUS
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the player character</summary>
	public long objInstID;

	/// <summary>Complete player character status</summary>
	public STATUS_PC status;
}
