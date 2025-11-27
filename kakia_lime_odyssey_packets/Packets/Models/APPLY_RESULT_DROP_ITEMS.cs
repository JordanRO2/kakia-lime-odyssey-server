/// <summary>
/// Application result indicating target dropped items (e.g., on death).
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: APPLY_RESULT_DROP_ITEMS
/// Size: 16 bytes (same as base APPLY_RESULT)
/// Used when an effect causes the target to drop items.
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct APPLY_RESULT_DROP_ITEMS
{
	/// <summary>Base application result</summary>
	public APPLY_RESULT baseResult;
}
