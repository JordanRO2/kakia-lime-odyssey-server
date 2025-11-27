/// <summary>
/// Container for multiple item inherit properties (max 25).
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: ITEM_INHERITS
/// Size: 300 bytes (25 * 12 bytes)
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Common;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ITEM_INHERITS
{
	/// <summary>Array of up to 25 item inherit properties</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 25)]
	public ITEM_INHERIT[] inherits;
}
