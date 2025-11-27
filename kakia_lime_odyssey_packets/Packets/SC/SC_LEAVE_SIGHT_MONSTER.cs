using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Notifies client that a monster has left their view range.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_LEAVE_SIGHT_MONSTER
/// Size: 10 bytes total
/// Inherits from: PACKET_SC_LEAVE_ZONEOBJ
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: __int64 objInstID (8 bytes) - Instance ID of leaving monster
/// Triggered by: Monster moving out of sight range or dying/despawning
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_LEAVE_SIGHT_MONSTER : IPacketFixed
{
	/// <summary>Instance ID of the monster leaving sight</summary>
	public long objInstID;
}
