/// <summary>
/// Server packet sent when collection increase rate changes.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_CHANGED_COLLECTION_INCREASE_RATE
/// Size: 22 bytes
/// Ordinal: 21495
/// Affects the bonus collection rate (additional items from gathering).
/// Uses float values with an extra field.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_CHANGED_COLLECTION_INCREASE_RATE : IPacketFixed
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the object</summary>
	public long objInstID;

	/// <summary>Current collection increase rate value</summary>
	public float current;

	/// <summary>Collection increase rate change amount (delta)</summary>
	public float update;

	/// <summary>Extra value (purpose unknown)</summary>
	public float extra;

	public const int Size = 22;
}
