using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet for NPC dialog message.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_TALKING
/// Size: 12 bytes total (variable-length packet)
/// Memory Layout (IDA):
/// - 0x00: PACKET_VAR header (4 bytes) - handled by IPacketVar
/// - 0x04: __int64 objInstID (8 bytes)
/// Note: Variable-length - dialog string data follows the fixed fields
/// Triggered by: CS_REQUEST_TALKING
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_TALKING : IPacketVar
{
	/// <summary>NPC instance ID (offset 0x04)</summary>
	public long objInstID;

	/// <summary>
	/// Dialog text string (not part of struct layout, handled separately)
	/// This field is not marshaled - it's for C# convenience only
	/// </summary>
	[field: NonSerialized]
	public string dialog { get; set; }
}

/// <summary>
/// Server->Client packet for NPC dialog choice menu.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_TALKING_CHOICE
/// Size: 12 bytes total (variable-length packet)
/// Memory Layout (IDA):
/// - 0x00: PACKET_VAR header (4 bytes) - handled by IPacketVar
/// - 0x04: __int64 objInstID (8 bytes)
/// Note: Variable-length - choice menu data follows the fixed fields
/// Triggered by: NPC dialog with choices
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_TALKING_CHOICE : IPacketVar
{
	/// <summary>NPC instance ID (offset 0x04)</summary>
	public long objInstID;

	// Variable length choice menu data follows (not included in struct)
}