/// <summary>
/// Application result indicating the skill missed its target.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: APPLY_RESULT_SKILL_MISSED
/// Size: 16 bytes (same as base APPLY_RESULT)
/// Used when a skill fails to hit the target.
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct APPLY_RESULT_SKILL_MISSED
{
	/// <summary>Base application result</summary>
	public APPLY_RESULT baseResult;
}
