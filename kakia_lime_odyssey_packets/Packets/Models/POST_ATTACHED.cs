using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

/// <summary>
/// Represents an item attached to a mail/post message (server->client).
/// Contains full item information including stats, durability, expiry, and inherits.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: POST_ATTACHED
/// Size: 324 bytes
/// Memory Layout (IDA):
/// - 0x00: int typeID (4 bytes)
/// - 0x04: int count (4 bytes)
/// - 0x08: int remainExpiryTime (4 bytes)
/// - 0x0C: int durability (4 bytes)
/// - 0x10: int mdurability (4 bytes)
/// - 0x14: int grade (4 bytes)
/// - 0x18: ITEM_INHERITS inherits (300 bytes)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct POST_ATTACHED
{
	/// <summary>Item type ID</summary>
	public int typeID;

	/// <summary>Item stack count</summary>
	public int count;

	/// <summary>Remaining expiry time in seconds (0 = no expiry)</summary>
	public int remainExpiryTime;

	/// <summary>Current durability</summary>
	public int durability;

	/// <summary>Maximum durability</summary>
	public int mdurability;

	/// <summary>Item grade/quality</summary>
	public int grade;

	/// <summary>Item inherit stats (300 bytes)</summary>
	public ITEM_INHERITS inherits;
}
