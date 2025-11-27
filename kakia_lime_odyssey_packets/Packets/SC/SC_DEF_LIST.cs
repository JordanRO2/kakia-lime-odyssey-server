using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet containing the list of active buffs/debuffs.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_DEF_LIST
/// Size: 4 bytes total (header only)
/// Memory Layout (IDA):
/// - 0x00: PACKET_VAR header (4 bytes) - handled by IPacketVar
/// Note: Variable-length array of DEF structs follows
/// Triggered by: CS_START_GAME, login
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_DEF_LIST : IPacketVar
{
	/// <summary>Number of DEF entries following</summary>
	public ushort count;

	/// <summary>Array of active buffs/debuffs (variable length)</summary>
	/// <remarks>Must be marshaled separately based on count</remarks>
	// public DEF[] defs; // Variable length array, handle in serialization
}
