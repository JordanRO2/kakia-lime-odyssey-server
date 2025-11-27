/// <summary>
/// Server->Client packet sent when player gains life job experience.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_GOT_LIFE_JOB_EXP
/// Size: 8 bytes (10 with PACKET_FIX header)
/// Triggered by: Experience gain from crafting/gathering
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_GOT_LIFE_JOB_EXP
{
	/// <summary>Current total life job experience</summary>
	public uint exp;

	/// <summary>Amount of experience gained</summary>
	public uint addExp;
}
