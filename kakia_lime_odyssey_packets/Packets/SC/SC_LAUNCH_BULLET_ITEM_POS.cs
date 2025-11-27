using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_LAUNCH_BULLET_ITEM_POS : IPacketFixed
{
	public long fromInstID;
	public FPOS toPos;
	public int typeID;
	public ushort useSP;
	public ushort useHP;
	public ushort useMP;
	public ushort useLP;
	public uint coolTime;
	public long bulletID;
	public uint tick;
	public float velocity;
}
