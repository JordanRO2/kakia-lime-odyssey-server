using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet to toggle helmet visibility.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_CHANGE_HELM_SHOWMODE
/// Size: 3 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: bool show (1 byte)
/// Response: SC_CHANGE_HELM_SHOWMODE
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_CHANGE_HELM_SHOWMODE : IPacketFixed
{
	/// <summary>True to show helmet, false to hide (offset 0x02)</summary>
	public bool show;
}
