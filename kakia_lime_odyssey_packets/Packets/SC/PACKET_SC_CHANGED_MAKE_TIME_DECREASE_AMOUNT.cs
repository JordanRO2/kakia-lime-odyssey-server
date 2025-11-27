/// <summary>
/// Server packet sent when crafting time decrease amount changes.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_CHANGED_MAKE_TIME_DECREASE_AMOUNT
/// Size: 22 bytes
/// Ordinal: 20536
/// Affects how much crafting time is reduced (from buffs, equipment, etc.).
/// Uses float values instead of int, with an extra field.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_CHANGED_MAKE_TIME_DECREASE_AMOUNT
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the object</summary>
	public long objInstID;

	/// <summary>Current make time decrease value</summary>
	public float current;

	/// <summary>Make time decrease change amount (delta)</summary>
	public float update;

	/// <summary>Extra value (purpose unknown)</summary>
	public float extra;
}
