using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

/// <summary>
/// Appearance structure for item-based projectile/bullet objects.
/// Simply wraps the base NPC appearance with no additional fields.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: APPEARANCE_BULLET_ITEM
/// Size: 60 bytes
/// Memory Layout (IDA):
/// - 0x00-0x3B: APPEARANCE_NPC (60 bytes) - Base NPC appearance (embedded, no field name)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct APPEARANCE_BULLET_ITEM
{
	public APPEARANCE_NPC appearance;
}
