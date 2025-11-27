using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Common;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct STUFF_MAKE_ITEM
{
	public int typeID;
	private int padding;
	public long count;
}
