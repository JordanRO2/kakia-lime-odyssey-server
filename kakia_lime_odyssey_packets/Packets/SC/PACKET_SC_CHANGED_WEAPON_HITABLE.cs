/// <summary>
/// Server packet sent when weapon hitable status changes.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_CHANGED_WEAPON_HITABLE
/// Size: 18 bytes
/// Ordinal: 21022
/// Controls whether weapons can hit (e.g., during certain states or restrictions).
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_CHANGED_WEAPON_HITABLE
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the object</summary>
	public long objInstID;

	/// <summary>Current weapon hitable value</summary>
	public int current;

	/// <summary>Weapon hitable change amount (delta)</summary>
	public int update;
}
