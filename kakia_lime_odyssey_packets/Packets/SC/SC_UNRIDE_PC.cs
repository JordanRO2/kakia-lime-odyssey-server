/// <summary>
/// Server->Client packet sent when a player or NPC dismounts from a pet/mount.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_UNRIDE_PC
/// Size: 10 bytes (2-byte header + 8 bytes data)
/// Triggered by: CS_UNRIDE_PC or forced dismount
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_UNRIDE_PC
{
	/// <summary>Object instance ID of the character dismounting</summary>
	public long objInstID;
}
