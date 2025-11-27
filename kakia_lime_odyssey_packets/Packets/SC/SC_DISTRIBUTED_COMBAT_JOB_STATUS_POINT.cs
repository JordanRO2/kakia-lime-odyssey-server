/// <summary>
/// Server->Client packet confirming combat job status point distribution.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_DISTRIBUTED_COMBAT_JOB_STATUS_POINT
/// Size: 16 bytes (18 with PACKET_FIX header)
/// Triggered by: CS_DISTRIBUTE_COMBAT_JOB_STATUS_POINT
/// Note: Returns new total stat values and remaining points after distribution
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_DISTRIBUTED_COMBAT_JOB_STATUS_POINT
{
	/// <summary>New total Strength (STR) stat value</summary>
	public ushort str;

	/// <summary>New total Intelligence (INL) stat value</summary>
	public ushort inl;

	/// <summary>New total Dexterity (DEX) stat value</summary>
	public ushort dex;

	/// <summary>New total Agility (AGI) stat value</summary>
	public ushort agi;

	/// <summary>New total Vitality (VIT) stat value</summary>
	public ushort vit;

	/// <summary>New total Spirit (SPI) stat value</summary>
	public ushort spi;

	/// <summary>New total Luck (LUK) stat value</summary>
	public ushort luk;

	/// <summary>Remaining status points after distribution</summary>
	public ushort point;
}
