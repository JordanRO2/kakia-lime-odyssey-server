/// <summary>
/// Server->Client packet containing the list of active buffs/debuffs.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_DEF_LIST
/// Size: Variable (4 byte header + array of DEF structs)
/// Triggered by: CS_START_GAME, login
///
/// This is a variable-length packet using PACKET_VAR header.
/// Format: [PACKET_VAR header (4 bytes)][count (ushort)][DEF array]
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_DEF_LIST
{
	/// <summary>Number of DEF entries following</summary>
	public ushort count;

	/// <summary>Array of active buffs/debuffs (variable length)</summary>
	/// <remarks>Must be marshaled separately based on count</remarks>
	// public DEF[] defs; // Variable length array, handle in serialization
}
