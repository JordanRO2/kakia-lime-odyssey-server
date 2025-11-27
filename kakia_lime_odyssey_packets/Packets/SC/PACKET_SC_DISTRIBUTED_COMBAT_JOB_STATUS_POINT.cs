/// <summary>
/// Server packet sent after combat job status points are distributed.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_DISTRIBUTED_COMBAT_JOB_STATUS_POINT
/// Size: 18 bytes
/// Ordinal: 2597
/// Confirms the stat distribution and shows updated values.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_DISTRIBUTED_COMBAT_JOB_STATUS_POINT
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Updated Strength stat value</summary>
	public ushort str;

	/// <summary>Updated Intelligence stat value</summary>
	public ushort inl;

	/// <summary>Updated Dexterity stat value</summary>
	public ushort dex;

	/// <summary>Updated Agility stat value</summary>
	public ushort agi;

	/// <summary>Updated Vitality stat value</summary>
	public ushort vit;

	/// <summary>Updated Spirit stat value</summary>
	public ushort spi;

	/// <summary>Updated Luck stat value</summary>
	public ushort luk;

	/// <summary>Remaining status points</summary>
	public ushort point;
}
