/// <summary>
/// Client->Server change party options (leader only).
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_CS_PARTY_CHANGE_OPTION
/// Size: 1 byte (3 with PACKET_FIX header)
/// Response: SC_PARTY_CHANGED_OPTION
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_PARTY_CHANGE_OPTION
{
	/// <summary>Party option type/flags</summary>
	public byte type;
}
