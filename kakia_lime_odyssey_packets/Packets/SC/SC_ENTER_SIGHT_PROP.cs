using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet sent when a prop (decoration/environment object) enters the player's view range.
/// Props are non-interactive environmental objects like decorations, furniture, or scenery.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_ENTER_SIGHT_PROP
/// Size: 101 bytes total
/// Memory Layout (IDA):
/// - 0x00-0x23: PACKET_SC_ENTER_ZONEOBJ (36 bytes) - Base zone object entry (embedded, no field name)
///   - 0x00: PACKET_VAR header (4 bytes, handled by framework)
///   - 0x04: __int64 objInstID (8 bytes)
///   - 0x0C: FPOS pos (12 bytes)
///   - 0x18: FPOS dir (12 bytes)
/// - 0x24-0x63: APPEARANCE_PROP appearance (64 bytes)
///   - 0x24-0x5F: APPEARANCE_NPC (60 bytes) - Base appearance
///   - 0x60-0x63: unsigned int typeID (4 bytes) - Prop type/subtype identifier
/// - 0x64: unsigned __int8 stopType (1 byte) - Movement stop type flag
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_ENTER_SIGHT_PROP : IPacketVar
{
	public SC_ENTER_ZONEOBJ enter_zone;
	public APPEARANCE_PROP appearance;
	public byte stopType;
}
