using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet notifying about player character appearance update.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_UPDATED_APPEARANCE_PC
/// Size: 162 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: __int64 objInstID (8 bytes)
/// - 0x0A: APPEARANCE_PC apperance (152 bytes)
/// Note: IDA has a typo "apperance" instead of "appearance"
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_UPDATED_APPEARANCE_PC : IPacketFixed
{
	/// <summary>Object instance ID (offset 0x02)</summary>
	public long objInstID;

	/// <summary>Player appearance data (offset 0x0A)</summary>
	public APPEARANCE_PC_KR appearance;
}
