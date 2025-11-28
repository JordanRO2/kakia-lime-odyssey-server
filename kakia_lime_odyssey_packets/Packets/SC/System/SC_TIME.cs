using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server notifies client of current game time.
/// Sent periodically to synchronize game time.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_TIME
/// Size: 5 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (ushort header) - 2 bytes (handled by IPacketFixed)
/// - 0x02: TIME_ time (hour, minute, second) - 3 bytes
/// Triggered by: Periodic server time updates
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_TIME : IPacketFixed
{
	/// <summary>Current game time (hour, minute, second)</summary>
	public TIME_ time;
}
