using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_LAUNCHED_BULLET_SKILL_RESULT_LIST : IPacketVar
{
	public uint skillTypeID;
	public long from;
	public uint tick;
}
