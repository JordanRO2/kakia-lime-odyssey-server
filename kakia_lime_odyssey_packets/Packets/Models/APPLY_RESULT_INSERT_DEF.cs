/// <summary>
/// Application result for adding a buff/debuff effect.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: APPLY_RESULT_INSERT_DEF
/// Size: 32 bytes (16-byte base + 16-byte DEF)
/// Used when a skill applies a buff or debuff to the target.
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct APPLY_RESULT_INSERT_DEF
{
	/// <summary>Base application result</summary>
	public APPLY_RESULT baseResult;

	/// <summary>The buff/debuff being applied</summary>
	public DEF def;
}
