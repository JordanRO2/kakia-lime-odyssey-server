using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet sent when player gains life job experience.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_GOT_LIFE_JOB_EXP
/// Size: 10 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: unsigned int exp (4 bytes)
/// - 0x06: unsigned int addExp (4 bytes)
/// Triggered by: Experience gain from crafting/gathering
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_GOT_LIFE_JOB_EXP : IPacketFixed
{
	/// <summary>Current total life job experience (offset 0x02)</summary>
	public uint exp;

	/// <summary>Amount of experience gained (offset 0x06)</summary>
	public uint addExp;
}
