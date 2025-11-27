/// <summary>
/// Application result indicating the skill was defended/blocked.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: APPLY_RESULT_SKILL_DEFENSED
/// Size: 16 bytes (same as base APPLY_RESULT)
/// Used when target blocks a skill with defense.
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct APPLY_RESULT_SKILL_DEFENSED
{
	/// <summary>Base application result</summary>
	public APPLY_RESULT baseResult;
}
