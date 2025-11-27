/// <summary>
/// Server->Client packet sent when a character levels up their life job.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_PC_LIFE_JOB_LEVEL_UP
/// Size: 15 bytes (17 with PACKET_FIX header)
/// Triggered by: Life job experience reaching threshold
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_PC_LIFE_JOB_LEVEL_UP
{
	/// <summary>Instance ID of the character leveling up</summary>
	public long objInstID;

	/// <summary>New life job level</summary>
	public byte lv;

	/// <summary>Current experience after level up</summary>
	public uint exp;

	/// <summary>New status points gained from level up</summary>
	public ushort statusPoint;
}
