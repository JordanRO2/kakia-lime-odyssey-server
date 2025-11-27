/// <summary>
/// Application result indicating the skill was resisted.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: APPLY_RESULT_SKILL_RESISTED
/// Size: 16 bytes (same as base APPLY_RESULT)
/// Used when target resists a skill effect.
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct APPLY_RESULT_SKILL_RESISTED
{
	/// <summary>Base application result</summary>
	public APPLY_RESULT baseResult;
}
