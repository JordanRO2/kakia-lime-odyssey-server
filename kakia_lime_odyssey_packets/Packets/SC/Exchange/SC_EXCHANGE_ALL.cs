/// <summary>
/// SC_ versions of exchange packets.
/// Note: Most PACKET_SC_EXCHANGE_* packets already don't have headers,
/// but we create SC_ versions for consistency and easier use.
/// </summary>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// IDA Verified: 2025-11-26
/// Exchange request confirmation (header only, no data).
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_REQUEST_EXCHANGE : IPacketFixed
{
	// No payload
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Sent to target player when someone requests to exchange.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_EXCHANGE_REQUESTED : IPacketFixed
{
	// No payload
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Start exchange window with target player.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_START_EXCHANGE : IPacketFixed
{
	/// <summary>Instance ID of the other player in the exchange</summary>
	public long target;
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Confirms item was successfully added to local player's exchange list.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_SUCCESS_ADD_ITEM_TO_EXCHANGE_LIST : IPacketFixed
{
	/// <summary>Item type identifier</summary>
	public int itemTypeID;

	/// <summary>Exchange slot number where item was placed</summary>
	public int slot;

	/// <summary>Number of items added to the exchange</summary>
	public long count;
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Shows the other player's added item with full details.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_ADDED_ITEM_TO_EXCHANGE_LIST : IPacketFixed
{
	/// <summary>Item type identifier</summary>
	public int itemTypeID;

	/// <summary>Exchange slot number</summary>
	public int slot;

	/// <summary>Number of items in the stack</summary>
	public long count;

	/// <summary>Current durability of the item</summary>
	public int durability;

	/// <summary>Maximum durability of the item</summary>
	public int mdurability;

	/// <summary>Item grade/quality level</summary>
	public int grade;

	/// <summary>Item enhancement, sockets, and inheritance data</summary>
	public ITEM_INHERITS inherits;
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Confirms item was successfully removed from local player's exchange list.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_SUCCESS_SUBTRACT_ITEM_FROM_EXCHANGE_LIST : IPacketFixed
{
	/// <summary>Exchange slot number</summary>
	public int slot;
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Notifies the other player that an item was removed from their partner's offer.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_SUBTRACTED_ITEM_FROM_EXCHANGE_LIST : IPacketFixed
{
	/// <summary>Item type identifier</summary>
	public int itemTypeID;

	/// <summary>Exchange slot number</summary>
	public int slot;

	/// <summary>Number of items removed from the exchange</summary>
	public long count;
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Notifies that the other player has marked ready.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_EXCHANGE_READY : IPacketFixed
{
	// No payload
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Notifies that both players are ready.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_EXCHANGE_READY_ALL : IPacketFixed
{
	// No payload
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Notifies that ready state was cancelled by one player.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_EXCHANGE_AGAIN : IPacketFixed
{
	// No payload
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Notifies that the other player has given final OK.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_EXCHANGE_OK : IPacketFixed
{
	// No payload
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Exchange completed successfully.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_EXCHANGE_SUCCESS : IPacketFixed
{
	// No payload
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Exchange failed or was cancelled.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_EXCHANGE_FAIL : IPacketFixed
{
	// No payload
}
