using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Notifies client that an entity has been resurrected.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_RESURRECTED
/// Size: 14 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: __int64 objInstID (8 bytes) - Instance ID of resurrected entity
/// - 0x0A: unsigned int hp (4 bytes) - HP after resurrection
/// Triggered by: Player resurrect, NPC resurrect ability
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_RESURRECTED : IPacketFixed
{
	/// <summary>Instance ID of the object that resurrected (offset 0x02)</summary>
	public long objInstID;

	/// <summary>HP amount after resurrection (offset 0x0A)</summary>
	public uint hp;
}
