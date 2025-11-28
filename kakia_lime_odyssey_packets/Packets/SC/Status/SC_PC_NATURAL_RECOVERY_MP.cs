using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet for natural MP (mana) recovery tick.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_PC_NATURAL_RECOVERY_MP
/// Size: 14 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: __int64 objInstID (8 bytes)
/// - 0x0A: unsigned __int16 current (2 bytes)
/// - 0x0C: unsigned __int16 difference (2 bytes)
/// Triggered by: Server-side periodic recovery tick
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_PC_NATURAL_RECOVERY_MP : IPacketFixed
{
	/// <summary>
	/// Object instance ID of the player character
	/// Offset: 0x02 (after PACKET_FIX header)
	/// Type: __int64 (long)
	/// </summary>
	public long objInstID;

	/// <summary>
	/// Current MP value after recovery
	/// Offset: 0x0A
	/// Type: unsigned __int16 (ushort)
	/// </summary>
	public ushort current;

	/// <summary>
	/// MP recovery amount (difference)
	/// Offset: 0x0C
	/// Type: unsigned __int16 (ushort)
	/// Indicates how much MP was recovered in this tick
	/// </summary>
	public ushort difference;
}
