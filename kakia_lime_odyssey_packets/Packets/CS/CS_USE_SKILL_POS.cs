using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// CS_USE_SKILL_POS - Client requests to use a skill at a position
/// IDA Verification: PACKET_CS_USE_SKILL_POS
/// Size: 19 bytes (2 byte header + 4 byte typeID + 12 byte pos + 1 byte combo)
/// Verified: 2025-11-26
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_USE_SKILL_POS : IPacketFixed
{
	public uint typeID;
	public FPOS pos;
	[MarshalAs(UnmanagedType.U1)]
	public bool combo;
}
