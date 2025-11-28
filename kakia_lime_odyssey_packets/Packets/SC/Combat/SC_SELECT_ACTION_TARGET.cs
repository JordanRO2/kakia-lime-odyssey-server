using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client notification that an entity selected an action target.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_SELECT_ACTION_TARGET
/// Size: 18 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: __int64 objInstID (8 bytes)
/// - 0x0A: __int64 targetInstID (8 bytes)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_SELECT_ACTION_TARGET : IPacketFixed
{
	/// <summary>Object instance ID selecting the target (offset 0x02)</summary>
	public long objInstID;

	/// <summary>Target object instance ID (offset 0x0A)</summary>
	public long targetInstID;
}
