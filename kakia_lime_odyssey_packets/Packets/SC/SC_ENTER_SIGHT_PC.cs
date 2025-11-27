using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet when another player character enters the view range.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_ENTER_SIGHT_PC
/// Size: 248 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_SC_ENTER_ZONEOBJ (36 bytes) - base zone object entry data
/// - 0x24: float deltaLookAtRadian (4 bytes)
/// - 0x28: APPEARANCE_PC appearance (152 bytes)
/// - 0xC0: int raceRelationState (4 bytes)
/// - 0xC4: unsigned __int8 stopType (1 byte)
/// - 0xC5: char[51] guildName (51 bytes)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_ENTER_SIGHT_PC : IPacketVar
{
	/// <summary>Base zone object entry data (offset 0x00)</summary>
	public SC_ENTER_ZONEOBJ enter_zone;

	/// <summary>Looking direction delta (offset 0x24)</summary>
	public float deltaLookAtRadian;

	/// <summary>Player appearance data (offset 0x28)</summary>
	public APPEARANCE_PC_KR appearance;

	/// <summary>Race relation state (offset 0xC0)</summary>
	public int raceRelationState;

	/// <summary>Stop state type (offset 0xC4)</summary>
	public byte stopType;

	/// <summary>Guild name (offset 0xC5)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 51)]
	public byte[] guildName;
}
