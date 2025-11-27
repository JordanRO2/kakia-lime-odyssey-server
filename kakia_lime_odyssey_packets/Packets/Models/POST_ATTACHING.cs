using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

/// <summary>
/// Represents an item to attach to a mail/post message (client->server).
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: POST_ATTACHING
/// Size: 16 bytes
/// Memory Layout (IDA):
/// - 0x00: int slot (4 bytes)
/// - 0x04: padding (4 bytes)
/// - 0x08: __int64 count (8 bytes)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct POST_ATTACHING
{
	/// <summary>Inventory slot index of the item to attach</summary>
	public int slot;

	/// <summary>Padding for alignment (4 bytes)</summary>
	private int _padding;

	/// <summary>Item count to attach</summary>
	public long count;
}
