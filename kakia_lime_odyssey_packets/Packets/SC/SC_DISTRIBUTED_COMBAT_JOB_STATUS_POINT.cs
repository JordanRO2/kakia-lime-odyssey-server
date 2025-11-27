using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet confirming combat job status point distribution.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_DISTRIBUTED_COMBAT_JOB_STATUS_POINT
/// Size: 18 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: unsigned __int16 str (2 bytes)
/// - 0x04: unsigned __int16 inl (2 bytes)
/// - 0x06: unsigned __int16 dex (2 bytes)
/// - 0x08: unsigned __int16 agi (2 bytes)
/// - 0x0A: unsigned __int16 vit (2 bytes)
/// - 0x0C: unsigned __int16 spi (2 bytes)
/// - 0x0E: unsigned __int16 luk (2 bytes)
/// - 0x10: unsigned __int16 point (2 bytes)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_DISTRIBUTED_COMBAT_JOB_STATUS_POINT : IPacketFixed
{
	/// <summary>New total Strength stat value (offset 0x02)</summary>
	public ushort str;

	/// <summary>New total Intelligence stat value (offset 0x04)</summary>
	public ushort inl;

	/// <summary>New total Dexterity stat value (offset 0x06)</summary>
	public ushort dex;

	/// <summary>New total Agility stat value (offset 0x08)</summary>
	public ushort agi;

	/// <summary>New total Vitality stat value (offset 0x0A)</summary>
	public ushort vit;

	/// <summary>New total Spirit stat value (offset 0x0C)</summary>
	public ushort spi;

	/// <summary>New total Luck stat value (offset 0x0E)</summary>
	public ushort luk;

	/// <summary>Remaining status points (offset 0x10)</summary>
	public ushort point;
}
