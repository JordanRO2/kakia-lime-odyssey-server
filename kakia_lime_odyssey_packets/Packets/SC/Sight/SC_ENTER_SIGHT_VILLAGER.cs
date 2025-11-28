using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Packet sent when a villager (NPC with special interaction capabilities) enters the player's view range.
/// Villagers are NPCs that can provide services like crafting, trading, or other specialized functions.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_ENTER_SIGHT_VILLAGER
/// Size: 105 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_SC_ENTER_ZONEOBJ (embedded, no field name) - 36 bytes
///   - 0x00: PACKET_VAR header (4 bytes, handled by framework)
///   - 0x04: __int64 objInstID
///   - 0x0C: FPOS pos
///   - 0x18: FPOS dir
/// - 0x24: APPEARANCE_VILLAGER appearance - 64 bytes
///   - 0x24: APPEARANCE_NPC (embedded) - 60 bytes
///     - 0x24: char[31] name
///     - 0x44: uint action
///     - 0x48: uint actionStartTick
///     - 0x4C: float scale
///     - 0x50: float transparent
///     - 0x54: uint modelTypeID
///     - 0x58: COLOR color (3 bytes)
///     - 0x5C: uint typeID
///   - 0x60: __int16 specialistType
/// - 0x64: int raceRelationState - 4 bytes
/// - 0x68: unsigned __int8 stopType - 1 byte
/// Note: Pack = 1 for proper alignment matching IDA structure (changed from Pack = 4)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_ENTER_SIGHT_VILLAGER : IPacketVar
{
	public SC_ENTER_ZONEOBJ enter_zone;
	public APPEARANCE_VILLAGER appearance;
	public int raceRelationState;
	public byte stopType;
}