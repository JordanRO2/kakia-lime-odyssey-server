using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_LEAVE_SIGHT_PC - Notifies nearby players that a character is leaving their sight
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_LEAVE_SIGHT_PC
{
	public long objInstID;
}
