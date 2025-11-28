using kakia_lime_odyssey_packets.Packets.Enums;
using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client login result packet.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_LOGIN_RESULT
/// Size: 10 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: PACKET_SC_LOGIN_RESULT::RESULT result (4 bytes)
/// - 0x06: int revision (4 bytes)
/// Triggered by: CS_LOGIN
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_LOGIN_RESULT : IPacketFixed
{
	/// <summary>Login result code (offset 0x02)</summary>
	public LOGIN_RESULT result;

	/// <summary>Server revision number, should match client revision 211 (offset 0x06)</summary>
	public int revision;
}
