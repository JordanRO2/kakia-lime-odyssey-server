/// <summary>
/// Client->Server packet to toggle helmet visibility.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_CHANGE_HELM_SHOWMODE
/// Size: 1 byte (3 with PACKET_FIX header)
/// Response: SC_CHANGE_HELM_SHOWMODE
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_CHANGE_HELM_SHOWMODE
{
	/// <summary>True to show helmet, false to hide</summary>
	public bool show;
}
