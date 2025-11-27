using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: ITEM_INHERIT @ 12 bytes
/// Represents a single inherited stat or property on an item.
/// Note: 3 bytes of padding after type field to align to 12 bytes.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
public struct ITEM_INHERIT
{
	public uint typeID;
	public int value;
	public byte type;
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: ITEM_INHERITS @ 300 bytes
/// Container for 25 item inherit slots (25 × 12 = 300 bytes).
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ITEM_INHERITS
{
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 25)]
	public ITEM_INHERIT[] inherits;
}
