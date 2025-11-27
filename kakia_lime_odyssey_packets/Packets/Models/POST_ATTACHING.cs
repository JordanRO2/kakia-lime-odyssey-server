using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: POST_ATTACHING @ 16 bytes
/// Represents an item to attach to a mail/post message (client->server).
/// Contains inventory slot and count. Note: 4 bytes padding between slot and count.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
public struct POST_ATTACHING
{
	public int slot;
	public long count;
}
