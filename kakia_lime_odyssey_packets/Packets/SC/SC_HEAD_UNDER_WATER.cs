using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_HEAD_UNDER_WATER - Broadcast that a player's head went underwater
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_HEAD_UNDER_WATER
{
	public long objInstID;
}
