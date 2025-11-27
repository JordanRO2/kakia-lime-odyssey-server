using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

/// <summary>
/// Appearance structure for prop (decoration/environment) objects.
/// Extends base NPC appearance with an additional type identifier.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: APPEARANCE_PROP
/// Size: 64 bytes
/// Memory Layout (IDA):
/// - 0x00-0x3B: APPEARANCE_NPC (60 bytes) - Base NPC appearance (embedded, no field name)
/// - 0x3C-0x3F: unsigned int typeID (4 bytes) - Prop type/subtype identifier
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct APPEARANCE_PROP
{
	public APPEARANCE_NPC appearance;
	public uint typeID;
}
