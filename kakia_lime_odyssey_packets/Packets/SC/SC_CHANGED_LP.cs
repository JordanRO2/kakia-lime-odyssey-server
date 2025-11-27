using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet sent when a character's LP (Life Points) changes.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_CHANGED_LP
/// Size: 18 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: __int64 objInstID (8 bytes)
/// - 0x0A: int current (4 bytes)
/// - 0x0E: int update (4 bytes)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_CHANGED_LP : IPacketFixed
{
	/// <summary>Instance ID of the character whose LP changed (offset 0x02)</summary>
	public long objInstID;

	/// <summary>Current LP value (offset 0x0A)</summary>
	public int current;

	/// <summary>LP change amount (offset 0x0E)</summary>
	public int update;
}
