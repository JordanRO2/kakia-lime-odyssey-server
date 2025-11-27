using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

/// <summary>
/// Integer-based position structure.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// Size: 12 bytes
/// Used for party member positions and other integer coordinate systems.
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct POS
{
	public int x;
	public int y;
	public float z;
}
