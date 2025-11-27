using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Notifies the client when another player character enters the view range.
/// Contains the player's position, appearance, and guild information.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_ENTER_SIGHT_PC
/// Size: 248 bytes total (including 4-byte PACKET_VAR header in SC_ENTER_ZONEOBJ)
/// Nested Structures:
/// - SC_ENTER_ZONEOBJ (36 bytes): Contains PACKET_VAR header, object ID, position, direction
/// - APPEARANCE_PC (152 bytes): Player appearance data (using APPEARANCE_PC_KR in C#)
/// Fields:
/// - enter_zone: Zone object entry data with position/direction
/// - deltaLookAtRadian: Looking direction delta (float, 4 bytes)
/// - appearance: Player appearance including name, race, job, equipment (152 bytes)
/// - raceRelationState: Race relation state (int, 4 bytes)
/// - stopType: Stop state type (unsigned __int8 -> byte, 1 byte)
/// - guildName: Guild name string (char[51], 51 bytes)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_ENTER_SIGHT_PC : IPacketVar
{
	public SC_ENTER_ZONEOBJ enter_zone;
	public float deltaLookAtRadian;
	public APPEARANCE_PC_KR appearance;
	public int raceRelationState;
	public byte stopType;
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 51)]
	public string guildName;
}
