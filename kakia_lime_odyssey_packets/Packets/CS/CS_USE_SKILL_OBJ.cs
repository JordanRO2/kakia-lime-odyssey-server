using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// CS_USE_SKILL_OBJ - Client requests to use a skill on a target object
/// IDA Verification: PACKET_CS_USE_SKILL_OBJ
/// Size: 15 bytes (2 byte header + 8 byte objInstID + 4 byte typeID + 1 byte combo)
/// Verified: 2025-11-26
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_USE_SKILL_OBJ : IPacketFixed
{
	public long objInstID;
	public uint typeID;
	[MarshalAs(UnmanagedType.U1)]
	public bool combo;
}
