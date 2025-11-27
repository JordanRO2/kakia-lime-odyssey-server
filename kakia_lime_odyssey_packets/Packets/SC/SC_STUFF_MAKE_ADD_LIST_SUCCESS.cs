using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_STUFF_MAKE_ADD_LIST_SUCCESS : IPacketFixed
{
	public uint typeID;
	public int successPercent;
	public uint makeTime;
	public ushort requestLP;
	public STUFF_MAKE_ITEMS resultItems;
	public STUFF_MAKE_SLOT addedItem;
}
