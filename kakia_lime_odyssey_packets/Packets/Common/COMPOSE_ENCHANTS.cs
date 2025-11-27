/// <summary>
/// Structure containing slots for item composition/enchantment materials.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: COMPOSE_ENCHANTS
/// Size: 20 bytes
/// Array of 5 inventory slot indices used for item composition or enchantment.
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Common;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct COMPOSE_ENCHANTS
{
	/// <summary>Array of 5 inventory slot indices for composition materials</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
	public int[] slots;
}
