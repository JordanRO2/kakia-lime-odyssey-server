using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

/// <summary>
/// Sight extension data for changing an entity's appearance to a player character.
/// Used when transforming/disguising into a PC appearance.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: SIGHT_EXTEND_CHANGE_APPEARANCE_TO_PC
/// Size: 153 bytes
/// Memory Layout (IDA):
/// - 0x00: SIGHT_EXTEND (1 byte) - Base sight extension header (embedded, no field name)
/// - 0x01: APPEARANCE_PC appearance (152 bytes) - Full PC appearance data
/// Total: 153 bytes (0x99)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SIGHT_EXTEND_CHANGE_APPEARANCE_TO_PC
{
	public SIGHT_EXTEND extend;
	public APPEARANCE_PC_KR appearance;
}
