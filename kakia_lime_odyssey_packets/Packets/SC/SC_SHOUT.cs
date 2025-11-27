using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client zone-wide shout message broadcast.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_SHOUT
/// Size: 38 bytes total (variable-length packet)
/// Memory Layout (IDA):
/// - 0x00: PACKET_VAR header (4 bytes) - handled by IPacketVar
/// - 0x04: __int64 instID (8 bytes)
/// - 0x0C: char[26] name (26 bytes)
/// Note: Variable-length message string follows
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_SHOUT : IPacketVar
{
	/// <summary>Shouter's object instance ID (offset 0x04)</summary>
	public long instID;

	/// <summary>Shouter's name (offset 0x0C)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
	public byte[] name;

	// Variable length message string follows (not included in struct)
}
