/// <summary>
/// Server packet sent when an entity's life/crafting status changes.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_CHANGED_LIFE_STATUS
/// Size: 22 bytes
/// Ordinal: 2622
/// Contains gathering and crafting-related status values.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_CHANGED_LIFE_STATUS
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the object</summary>
	public long objInstID;

	/// <summary>Collection success rate</summary>
	public int collectSucessRate;

	/// <summary>Collection increase rate</summary>
	public int collectionIncreaseRate;

	/// <summary>Crafting time decrease amount</summary>
	public int makeTimeDecreaseAmount;
}
