using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet containing the list of characters for the logged-in account.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_PC_LIST
/// Size: 5 bytes total (header + count)
/// Memory Layout (IDA):
/// - 0x00: PACKET_VAR header (4 bytes) - handled by IPacketVar
/// - 0x04: unsigned __int8 count (1 byte)
/// Note: Variable-length array of CLIENT_PC (220 bytes each) follows
/// Triggered by: CS_LOGIN (after successful authentication)
/// Handler: CSeparateHandler::sc_pc_list @ 0x5b304c
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_PC_LIST : IPacketVar
{
	/// <summary>Number of characters in the list (offset 0x04)</summary>
	public byte count;

	// Note: Variable-length array of CLIENT_PC follows, handled separately
}
