/// <summary>
/// Server->Client notification that party options changed.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_PARTY_CHANGED_OPTION
/// Size: 1 byte (3 with PACKET_FIX header)
/// Triggered by: CS_PARTY_CHANGE_OPTION
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_PARTY_CHANGED_OPTION
{
	/// <summary>New party option type/flags</summary>
	public byte type;
}
