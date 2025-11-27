using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// CS_USE_SKILL_SELF - Client requests to use a skill on self
/// IDA Verification: PACKET_CS_USE_SKILL_SELF
/// Size: 7 bytes (2 byte header + 4 byte typeID + 1 byte combo)
/// Verified: 2025-11-26
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_USE_SKILL_SELF : IPacketFixed
{
	public uint typeID;
	[MarshalAs(UnmanagedType.U1)]
	public bool combo;
}