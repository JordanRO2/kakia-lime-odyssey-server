/// <summary>
/// Application result for removing a buff/debuff effect.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: APPLY_RESULT_REMOVE_DEF
/// Size: 24 bytes (16-byte base + 4-byte instID + 4 bytes padding)
/// Used when a skill removes a buff or debuff from the target.
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct APPLY_RESULT_REMOVE_DEF
{
	/// <summary>Base application result</summary>
	public APPLY_RESULT baseResult;

	/// <summary>Instance ID of the buff/debuff to remove</summary>
	public uint instID;

	/// <summary>Padding (4 bytes) to maintain alignment</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
	private byte[] _padding;
}
