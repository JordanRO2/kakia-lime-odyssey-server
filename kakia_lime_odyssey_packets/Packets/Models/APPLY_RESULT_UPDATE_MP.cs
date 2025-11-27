/// <summary>
/// Application result for MP (Mana Points) update effects.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: APPLY_RESULT_UPDATE_MP
/// Size: 24 bytes (16-byte base + 4-byte val + 1-byte critical + 3 bytes padding)
/// Used when skills or items modify MP values.
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct APPLY_RESULT_UPDATE_MP
{
	/// <summary>Base application result</summary>
	public APPLY_RESULT baseResult;

	/// <summary>MP change value</summary>
	public int val;

	/// <summary>Whether this was a critical effect</summary>
	public bool critical;

	/// <summary>Padding (3 bytes) to maintain alignment</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	private byte[] _padding;
}
