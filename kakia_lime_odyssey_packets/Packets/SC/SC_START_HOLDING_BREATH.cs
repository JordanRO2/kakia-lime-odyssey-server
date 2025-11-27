using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet indicating breath-holding has started (underwater).
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_START_HOLDING_BREATH
/// Size: 14 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: unsigned int capacity (4 bytes)
/// - 0x06: unsigned int current (4 bytes)
/// - 0x0A: unsigned int value (4 bytes)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_START_HOLDING_BREATH : IPacketFixed
{
	/// <summary>Maximum breath capacity (offset 0x02)</summary>
	public uint capacity;

	/// <summary>Current breath amount (offset 0x06)</summary>
	public uint current;

	/// <summary>Breath depletion rate value (offset 0x0A)</summary>
	public uint value;
}
