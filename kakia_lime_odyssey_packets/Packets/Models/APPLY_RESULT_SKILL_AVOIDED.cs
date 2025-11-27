/// <summary>
/// Application result indicating the skill was avoided/evaded.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: APPLY_RESULT_SKILL_AVOIDED
/// Size: 16 bytes (same as base APPLY_RESULT)
/// Used when target evades/avoids a skill attack.
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct APPLY_RESULT_SKILL_AVOIDED
{
	/// <summary>Base application result</summary>
	public APPLY_RESULT baseResult;
}
