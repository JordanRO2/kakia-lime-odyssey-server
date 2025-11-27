using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

/// <summary>
/// Appearance structure for merchant NPCs.
/// Extends base NPC appearance with a specialist type identifier.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: APPEARANCE_MERCHANT
/// Size: 64 bytes
/// Memory Layout (IDA):
/// - 0x00-0x3B: APPEARANCE_NPC (60 bytes) - Base NPC appearance (embedded, no field name)
/// - 0x3C-0x3D: __int16 specialistType (2 bytes) - Merchant specialist type/category
/// Note: Pack = 1 matches IDA structure layout (changed from Pack = 4)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct APPEARANCE_MERCHANT
{
	public APPEARANCE_NPC appearance;
	public short specialistType;
}
