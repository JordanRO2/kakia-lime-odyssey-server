using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet sent when a player dismounts from a pet/mount.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_UNRIDE_PC
/// Size: 10 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: __int64 objInstID (8 bytes)
/// Triggered by: CS_UNRIDE_PC or forced dismount
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_UNRIDE_PC : IPacketFixed
{
	/// <summary>Object instance ID of the character dismounting (offset 0x02)</summary>
	public long objInstID;
}
