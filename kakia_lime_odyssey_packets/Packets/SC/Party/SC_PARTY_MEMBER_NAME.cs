using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client update of party member's name.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_PARTY_MEMBER_NAME
/// Size: 32 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: unsigned int idx (4 bytes)
/// - 0x06: char[26] name (26 bytes)
/// Triggered by: Member name change
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_PARTY_MEMBER_NAME : IPacketFixed
{
	/// <summary>Party member index (offset 0x02)</summary>
	public uint idx;

	/// <summary>New character name (offset 0x06)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
	public byte[] name;
}
