using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet to transform entity to NPC appearance.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_CHANGE_TO_NPC_SHAPE
/// Size: 14 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: __int64 objInstID (8 bytes)
/// - 0x0A: int modelTypeID (4 bytes)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_CHANGE_TO_NPC_SHAPE : IPacketFixed
{
	/// <summary>Entity instance ID being transformed (offset 0x02)</summary>
	public long objInstID;

	/// <summary>NPC model type ID to transform into (offset 0x0A)</summary>
	public int modelTypeID;
}
