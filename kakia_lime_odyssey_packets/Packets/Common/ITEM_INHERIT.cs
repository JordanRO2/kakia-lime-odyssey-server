/// <summary>
/// Item inherit property structure for crafting and item generation.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: ITEM_INHERIT
/// Size: 12 bytes
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Common;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ITEM_INHERIT
{
	/// <summary>Type ID of the inherit property</summary>
	public uint typeID;

	/// <summary>Value of the inherit property</summary>
	public int value;

	/// <summary>Type classification of the inherit</summary>
	public byte type;

	/// <summary>Padding bytes to match 12-byte alignment (3 bytes padding after type)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	private byte[] padding;
}
