using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Confirms that a guild was successfully created.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_GUILD_CREATED
/// Size: 2 bytes total (header only)
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// Triggered by: CS_GUILD_CREATE success
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_GUILD_CREATED : IPacketFixed
{
	// Empty packet - header only
}
