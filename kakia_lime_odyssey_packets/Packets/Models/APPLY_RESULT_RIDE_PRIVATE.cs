/// <summary>
/// Specific application result for mount/pet riding effects.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: APPLY_RESULT_RIDE_PRIVATE
/// Size: 24 bytes (16-byte APPLY_RESULT base + 4-byte itemTypeID + 4 bytes padding)
/// Used in skill/item results that grant mount/riding capability.
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct APPLY_RESULT_RIDE_PRIVATE
{
	/// <summary>Base application result</summary>
	public APPLY_RESULT baseResult;

	/// <summary>Mount/pet item type ID</summary>
	public int itemTypeID;

	/// <summary>Padding (4 bytes) to maintain alignment</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
	private byte[] _padding;
}
