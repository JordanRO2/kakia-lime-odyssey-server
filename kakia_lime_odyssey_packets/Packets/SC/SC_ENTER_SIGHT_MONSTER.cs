using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Sent when a monster enters the player's visibility range.
/// Provides complete appearance and state information for the monster.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_ENTER_SIGHT_MONSTER
/// Total Size: 105 bytes
/// Memory Layout (IDA):
/// - 0x00-0x23: PACKET_SC_ENTER_ZONEOBJ (36 bytes) - Base zone object entry (instID, pos, dir)
/// - 0x24-0x63: APPEARANCE_MONSTER (64 bytes) - Monster appearance data
///   - 0x24-0x5F: APPEARANCE_NPC (60 bytes) - Base NPC appearance (name, action, scale, model, color, typeID)
///   - 0x60: bool aggresive (1 byte) - Whether monster is aggressive
///   - 0x61: bool shineWhenHitted (1 byte) - Visual effect when hit
/// - 0x64-0x67: int raceRelationState (4 bytes) - Race relation status (hostile, neutral, friendly)
/// - 0x68: unsigned __int8 stopType (1 byte) - Stop/movement state type
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_ENTER_SIGHT_MONSTER : IPacketVar
{
	public SC_ENTER_ZONEOBJ enter_zone;
	public APPEARANCE_MONSTER appearance;
	public int raceRelationState;
	public byte stopType;
}
