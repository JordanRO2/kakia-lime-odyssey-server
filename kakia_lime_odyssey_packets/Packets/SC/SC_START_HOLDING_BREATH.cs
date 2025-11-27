using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_START_HOLDING_BREATH : IPacketFixed
{
	public uint capacity;
	public uint current;
	public uint value;
}
