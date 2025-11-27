using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

/// <summary>
/// Monster appearance data extending base NPC appearance.
/// Contains monster-specific visual and behavioral flags.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: APPEARANCE_MONSTER
/// Size: 64 bytes
/// Memory Layout (IDA):
/// - 0x00-0x3B: APPEARANCE_NPC (60 bytes) - Base appearance structure
/// - 0x3C: bool aggresive (1 byte) - Monster aggression flag
/// - 0x3D: bool shineWhenHitted (1 byte) - Visual shine effect on hit
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct APPEARANCE_MONSTER
{
	public APPEARANCE_NPC appearance;
	[MarshalAs(UnmanagedType.U1)]
	public bool aggresive;
	[MarshalAs(UnmanagedType.U1)]
	public bool shineWhenHitted;
}
