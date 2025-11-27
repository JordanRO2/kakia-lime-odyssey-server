using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server notifies client about mission zone creation progress.
/// Provides queue position and zone creation statistics.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_CREATING_MISSION_ZONE
/// Size: 18 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (ushort header) - 2 bytes (handled by IPacketFixed)
/// - 0x02: int order - 4 bytes (queue position)
/// - 0x06: int waiting - 4 bytes (number waiting in queue)
/// - 0x0A: int created - 4 bytes (number of zones created)
/// - 0x0E: int createLimit - 4 bytes (max zones that can be created)
/// Triggered by: Mission zone creation request
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_CREATING_MISSION_ZONE : IPacketFixed
{
	/// <summary>Player's position in queue</summary>
	public int order;

	/// <summary>Number of players waiting in queue</summary>
	public int waiting;

	/// <summary>Number of zones currently created</summary>
	public int created;

	/// <summary>Maximum number of zones that can be created</summary>
	public int createLimit;
}
