using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

/// <summary>
/// Appearance data for villager NPCs (NPCs with specialized interaction capabilities).
/// Extends base NPC appearance with specialist type information.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: APPEARANCE_VILLAGER
/// Size: 64 bytes total
/// Memory Layout (IDA):
/// - 0x00-0x3B: APPEARANCE_NPC (embedded, no field name) - 60 bytes
/// - 0x3C-0x3D: __int16 specialistType - 2 bytes
/// Note: Pack = 1 for proper alignment matching IDA structure (changed from Pack = 4)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct APPEARANCE_VILLAGER
{
	public APPEARANCE_NPC appearance;
	public short specialistType;
}
