/// <summary>
/// Server packet sent when collection success rate changes.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_CHANGED_COLLECT_SUCESS_RATE
/// Size: 22 bytes
/// Ordinal: 21441
/// Affects the success rate for gathering/collecting resources.
/// Uses float values with an extra field.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_CHANGED_COLLECT_SUCCESS_RATE
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the object</summary>
	public long objInstID;

	/// <summary>Current collect success rate value</summary>
	public float current;

	/// <summary>Collect success rate change amount (delta)</summary>
	public float update;

	/// <summary>Extra value (purpose unknown)</summary>
	public float extra;
}
