using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: POST_ATTACHED @ 324 bytes
/// Represents an item attached to a mail/post message (server->client).
/// Contains full item information including stats, durability, expiry, and inherits.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct POST_ATTACHED
{
	public int typeID;
	public int count;
	public int remainExpiryTime;
	public int durability;
	public int mdurability;
	public int grade;
	public ITEM_INHERITS inherits;
}
