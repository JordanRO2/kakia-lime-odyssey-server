/// <summary>
/// Server->Client packet sent when a character's LP (Life Points) changes.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_CHANGED_LP
/// Size: 16 bytes (18 with PACKET_FIX header)
/// Triggered by: LP consumption, regeneration, or LP modification from life job activities
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_CHANGED_LP
{
	/// <summary>Instance ID of the character whose LP changed</summary>
	public long objInstID;

	/// <summary>Current LP value</summary>
	public int current;

	/// <summary>LP change amount (positive for gain, negative for consumption)</summary>
	public int update;
}
