using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet sent when a character levels up their life job.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_PC_LIFE_JOB_LEVEL_UP
/// Size: 17 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: __int64 objInstID (8 bytes)
/// - 0x0A: unsigned __int8 lv (1 byte)
/// - 0x0B: unsigned int exp (4 bytes)
/// - 0x0F: unsigned __int16 statusPoint (2 bytes)
/// Triggered by: Life job experience reaching threshold
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_PC_LIFE_JOB_LEVEL_UP : IPacketFixed
{
	/// <summary>Instance ID of the character leveling up (offset 0x02)</summary>
	public long objInstID;

	/// <summary>New life job level (offset 0x0A)</summary>
	public byte lv;

	/// <summary>Current experience after level up (offset 0x0B)</summary>
	public uint exp;

	/// <summary>New status points gained from level up (offset 0x0F)</summary>
	public ushort statusPoint;
}
