/// <summary>
/// Server->Client notification that an object (mob/container) has become lootable.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_LOOTABLE
/// Size: 10 bytes (8 bytes + 2 byte PACKET_FIX header)
/// Fields:
///   - objInstID: Instance ID of the lootable object (8 bytes at offset 0x02)
/// Triggered by: Mob death or container activation
/// </remarks>
using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_LOOTABLE : IPacketFixed
{
	/// <summary>Instance ID of the lootable object (mob/container)</summary>
	public long objInstID;
}
