/// <summary>
/// Server packet sent when skill usability status changes.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_CHANGED_SKILL_USABLE
/// Size: 18 bytes
/// Ordinal: 22493
/// Controls whether skills can be used (e.g., due to silences or restrictions).
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_CHANGED_SKILL_USABLE
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the object</summary>
	public long objInstID;

	/// <summary>Current skill usable value</summary>
	public int current;

	/// <summary>Skill usable change amount (delta)</summary>
	public int update;
}
