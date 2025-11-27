using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet indicating helmet visibility has changed.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_CHANGE_HELM_SHOWMODE
/// Size: 11 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: __int64 instID (8 bytes)
/// - 0x0A: bool show (1 byte)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_CHANGE_HELM_SHOWMODE : IPacketFixed
{
	/// <summary>Entity instance ID (offset 0x02)</summary>
	public long instID;

	/// <summary>Whether helmet is shown (offset 0x0A)</summary>
	public bool show;
}
