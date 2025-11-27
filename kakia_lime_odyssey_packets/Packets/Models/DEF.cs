using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

/// <summary>
/// Buff/debuff effect definition structure.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// Size: 16 bytes
/// Represents active buffs, debuffs, or status effects.
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct DEF
{
	/// <summary>Instance ID of this effect</summary>
	public uint instID;

	/// <summary>Type ID from skill/effect data</summary>
	public uint typeID;

	/// <summary>Level of the effect</summary>
	public ushort lv;

	/// <summary>Duration time remaining in milliseconds</summary>
	public int durTime;
}
