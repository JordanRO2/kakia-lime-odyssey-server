using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

/// <summary>
/// Sight extension data for changing an entity's appearance to an NPC.
/// Used when transforming/disguising into an NPC appearance.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: SIGHT_EXTEND_CHANGE_APPEARANCE_TO_NPC
/// Size: 61 bytes
/// Memory Layout (IDA):
/// - 0x00: SIGHT_EXTEND (1 byte) - Base sight extension header (embedded, no field name)
/// - 0x01: APPEARANCE_NPC appearance (60 bytes) - Full NPC appearance data
/// Total: 61 bytes (0x3D)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SIGHT_EXTEND_CHANGE_APPEARANCE_TO_NPC
{
	public SIGHT_EXTEND extend;
	public APPEARANCE_NPC appearance;
}
