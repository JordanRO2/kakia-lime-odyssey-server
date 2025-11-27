/// <summary>
/// Server->Client packet sent when a character's MP changes.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_CHANGED_MP
/// Size: 16 bytes (18 with PACKET_FIX header)
/// Triggered by: MP consumption, regeneration, or MP modification
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_CHANGED_MP
{
	/// <summary>Instance ID of the character whose MP changed</summary>
	public long objInstID;

	/// <summary>Current MP value</summary>
	public int current;

	/// <summary>MP change amount (positive for gain, negative for consumption)</summary>
	public int update;
}
