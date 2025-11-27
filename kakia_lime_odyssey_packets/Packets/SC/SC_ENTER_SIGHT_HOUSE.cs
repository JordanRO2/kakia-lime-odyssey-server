using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet sent when a house/building object enters the player's view range.
/// Houses are player-owned or static building structures in the game world.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_ENTER_SIGHT_HOUSE
/// Size: 96 bytes total
/// Memory Layout (IDA):
/// - 0x00-0x23: PACKET_SC_ENTER_ZONEOBJ (36 bytes) - Base zone object entry (embedded, no field name)
///   - 0x00: PACKET_VAR header (4 bytes, handled by framework)
///   - 0x04: __int64 objInstID (8 bytes)
///   - 0x0C: FPOS pos (12 bytes)
///   - 0x18: FPOS dir (12 bytes)
/// - 0x24-0x5F: APPEARANCE_HOUSE appearance (60 bytes)
///   - Contains APPEARANCE_NPC (60 bytes): name, action, scale, modelTypeID, color, typeID
/// Note: Unlike other ENTER_SIGHT packets, this one has NO stopType field at the end
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_ENTER_SIGHT_HOUSE : IPacketVar
{
	public SC_ENTER_ZONEOBJ enter_zone;
	public APPEARANCE_HOUSE appearance;
}
