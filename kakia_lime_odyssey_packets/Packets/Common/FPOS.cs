/// <summary>
/// Structure representing a 3D floating-point position.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: FPOS
/// Size: 12 bytes
/// Used for world coordinates in 3D space.
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Common;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct FPOS
{
	/// <summary>X coordinate</summary>
	public float x;

	/// <summary>Y coordinate</summary>
	public float y;

	/// <summary>Z coordinate</summary>
	public float z;
}
