using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Common;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct STUFF_MAKE_ITEMS
{
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
	public STUFF_MAKE_ITEM[] stuffMakeItems;
}
