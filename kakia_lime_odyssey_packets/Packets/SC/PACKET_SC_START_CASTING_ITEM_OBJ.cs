/// <summary>
/// Server packet sent when an entity starts casting an item targeting another object.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_START_CASTING_ITEM_OBJ
/// Size: 26 bytes
/// Ordinal: 2662
/// Initiates item casting animation and cast bar targeting an object.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_START_CASTING_ITEM_OBJ
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the caster</summary>
	public long fromInstID;

	/// <summary>Instance ID of the target object</summary>
	public long targetInstID;

	/// <summary>Item type ID being cast</summary>
	public int typeID;

	/// <summary>Total casting time in milliseconds</summary>
	public uint castTime;
}
