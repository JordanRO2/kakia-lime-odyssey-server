/// <summary>
/// Base structure for skill/item application results.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: APPLY_RESULT
/// Size: 16 bytes (with padding)
/// Used as base for various skill and item effect results.
/// Note: 7 bytes of padding exist between type and targetInstID.
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct APPLY_RESULT
{
	/// <summary>Type of application result</summary>
	public byte type;

	/// <summary>Padding (7 bytes)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
	private byte[] _padding;

	/// <summary>Target object instance ID</summary>
	public long targetInstID;
}
