/// <summary>
/// SC_ versions of item casting packets without PACKET_FIX header.
/// These are used for server-side packet sending.
/// </summary>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_START_CASTING_ITEM_OBJ (26 bytes total)
/// Initiates item casting animation and cast bar targeting an object.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_START_CASTING_ITEM_OBJ : IPacketFixed
{
	/// <summary>Instance ID of the caster</summary>
	public long fromInstID;

	/// <summary>Instance ID of the target object</summary>
	public long targetInstID;

	/// <summary>Item type ID being cast</summary>
	public int typeID;

	/// <summary>Total casting time in milliseconds</summary>
	public uint castTime;
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_START_CASTING_ITEM_POS (30 bytes total)
/// Initiates item casting animation and cast bar targeting a world position.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_START_CASTING_ITEM_POS : IPacketFixed
{
	/// <summary>Instance ID of the caster</summary>
	public long fromInstID;

	/// <summary>Target position in world coordinates</summary>
	public FPOS pos;

	/// <summary>Item type ID being cast</summary>
	public int typeID;

	/// <summary>Total casting time in milliseconds</summary>
	public uint castTime;
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_START_CASTING_ITEM_SLOT (22 bytes total)
/// Initiates item casting animation and cast bar targeting an inventory slot.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_START_CASTING_ITEM_SLOT : IPacketFixed
{
	/// <summary>Instance ID of the caster</summary>
	public long fromInstID;

	/// <summary>Target inventory slot index</summary>
	public int targetSlot;

	/// <summary>Item type ID being cast</summary>
	public int typeID;

	/// <summary>Total casting time in milliseconds</summary>
	public uint castTime;
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_DELAYED_CASTING_ITEM (14 bytes total)
/// Updates the client on remaining cast time for an item being cast.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_DELAYED_CASTING_ITEM : IPacketFixed
{
	/// <summary>Instance ID of the caster</summary>
	public long instID;

	/// <summary>Remaining casting time in milliseconds</summary>
	public uint remainTime;
}
