/// <summary>
/// Client packet to distribute combat job status points.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_DISTRIBUTE_COMBAT_JOB_STATUS_POINT
/// Size: 16 bytes
/// Ordinal: 2611
/// Allows player to allocate combat job stat points to STR, INT, DEX, AGI, VIT, SPI, LUK.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_DISTRIBUTE_COMBAT_JOB_STATUS_POINT
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Points to distribute to Strength stat</summary>
	public ushort supplyedSTR;

	/// <summary>Points to distribute to Intelligence stat</summary>
	public ushort supplyedINL;

	/// <summary>Points to distribute to Dexterity stat</summary>
	public ushort supplyedDEX;

	/// <summary>Points to distribute to Agility stat</summary>
	public ushort supplyedAGI;

	/// <summary>Points to distribute to Vitality stat</summary>
	public ushort supplyedVIT;

	/// <summary>Points to distribute to Spirit stat</summary>
	public ushort supplyedSPI;

	/// <summary>Points to distribute to Luck stat</summary>
	public ushort supplyedLUK;
}
