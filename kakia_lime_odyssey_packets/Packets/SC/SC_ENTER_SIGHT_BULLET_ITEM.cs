using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet sent when an item-based projectile/bullet enters the player's view range.
/// Item bullets are projectiles created by throwable items (e.g., thrown weapons, consumable projectiles).
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_ENTER_SIGHT_BULLET_ITEM
/// Size: 97 bytes total
/// Memory Layout (IDA):
/// - 0x00-0x23: PACKET_SC_ENTER_ZONEOBJ (36 bytes) - Base zone object entry (embedded, no field name)
///   - 0x00: PACKET_VAR header (4 bytes, handled by framework)
///   - 0x04: __int64 objInstID (8 bytes)
///   - 0x0C: FPOS pos (12 bytes)
///   - 0x18: FPOS dir (12 bytes)
/// - 0x24-0x5F: APPEARANCE_BULLET_ITEM appearance (60 bytes)
///   - Contains APPEARANCE_NPC (60 bytes): name, action, scale, modelTypeID, color, typeID
/// - 0x60: unsigned __int8 stopType (1 byte) - Movement stop type flag
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_ENTER_SIGHT_BULLET_ITEM : IPacketVar
{
	public SC_ENTER_ZONEOBJ enter_zone;
	public APPEARANCE_BULLET_ITEM appearance;
	public byte stopType;
}
