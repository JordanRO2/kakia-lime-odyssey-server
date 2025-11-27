/// <summary>
/// Server packet sent when a player or entity's HP changes.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_CHANGED_HP
/// Size: 26 bytes
/// Ordinal: 2614
/// This packet includes information about who caused the HP change (damage dealer or healer).
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_CHANGED_HP
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the object whose HP changed</summary>
	public long objInstID;

	/// <summary>Current HP value</summary>
	public int current;

	/// <summary>HP change amount (delta)</summary>
	public int update;

	/// <summary>Instance ID of the entity that caused the change (attacker/healer)</summary>
	public long fromID;
}
