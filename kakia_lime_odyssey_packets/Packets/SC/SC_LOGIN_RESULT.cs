/// <summary>
/// Server->Client login result packet.
/// </summary>
/// <remarks>
/// IDA Verified: Yes
/// IDA Struct: PACKET_SC_LOGIN_RESULT
/// Size: 8 bytes (10 with PACKET_FIX header)
/// Triggered by: CS_LOGIN
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Enums;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_LOGIN_RESULT
{
	/// <summary>Login result code</summary>
	public LOGIN_RESULT result;

	/// <summary>Server revision number (should match client revision 211)</summary>
	public int revision;
}
