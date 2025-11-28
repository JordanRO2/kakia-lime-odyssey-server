using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client notification that party options changed.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_PARTY_CHANGED_OPTION
/// Size: 3 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: unsigned __int8 type (1 byte)
/// Triggered by: CS_PARTY_CHANGE_OPTION
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_PARTY_CHANGED_OPTION : IPacketFixed
{
	/// <summary>New party option type/flags (offset 0x02)</summary>
	public byte type;
}
