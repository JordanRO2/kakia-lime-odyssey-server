using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_SLOT_ITEM_INFO : IPacketFixed
{
	public ITEM_SLOT slot;
	public int typeID;
	public long count;
	public int durability;
	public int mdurability;
	public int grade;
	public int remainExpiryTime;
	public ITEM_INHERITS inherits;
}
