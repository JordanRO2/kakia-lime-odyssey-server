/// <summary>
/// Client->Server packet to distribute combat job status points.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_DISTRIBUTE_COMBAT_JOB_STATUS_POINT
/// Size: 14 bytes (16 with PACKET_FIX header)
/// Response: SC_DISTRIBUTED_COMBAT_JOB_STATUS_POINT
/// Note: Player distributes stat points gained from combat job level ups
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct CS_DISTRIBUTE_COMBAT_JOB_STATUS_POINT
{
	/// <summary>Points to add to Strength (STR) stat</summary>
	public ushort supplyedSTR;

	/// <summary>Points to add to Intelligence (INL) stat</summary>
	public ushort supplyedINL;

	/// <summary>Points to add to Dexterity (DEX) stat</summary>
	public ushort supplyedDEX;

	/// <summary>Points to add to Agility (AGI) stat</summary>
	public ushort supplyedAGI;

	/// <summary>Points to add to Vitality (VIT) stat</summary>
	public ushort supplyedVIT;

	/// <summary>Points to add to Spirit (SPI) stat</summary>
	public ushort supplyedSPI;

	/// <summary>Points to add to Luck (LUK) stat</summary>
	public ushort supplyedLUK;
}
