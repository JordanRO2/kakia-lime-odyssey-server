/// <summary>
/// Server->Client packet sent when a character's HP changes.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_CHANGED_HP
/// Size: 24 bytes (26 with PACKET_FIX header)
/// Triggered by: Damage, healing, or HP modification
/// Note: Uses PACKET_CHANGED_STATUS base structure + fromID
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_CHANGED_HP
{
	/// <summary>Instance ID of the character whose HP changed</summary>
	public long objInstID;

	/// <summary>Current HP value</summary>
	public int current;

	/// <summary>HP change amount (positive for heal, negative for damage)</summary>
	public int update;

	/// <summary>Instance ID of the entity that caused the change (attacker/healer)</summary>
	public long fromID;
}
