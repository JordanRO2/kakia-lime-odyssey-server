using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_PC_NATURAL_RECOVERY_SP - Server to Client SP natural recovery packet
/// Total size: 14 bytes (including 2-byte PACKET_FIX header)
/// IDA verified: 2025-11-26
/// Structure name in client: PACKET_SC_PC_NATURAL_RECOVERY_SP
/// </summary>
/// <remarks>
/// This packet is sent periodically to update the client with natural SP (Stamina Point) regeneration.
/// Triggered by: Server-side periodic natural recovery tick
/// The current field contains the new SP value, difference shows how much was recovered.
/// IDA Details:
/// - Total size: 14 bytes
/// - Member count: 4 (PACKET_FIX header + objInstID + current + difference)
/// - Offset 0x02: __int64 objInstID (8 bytes)
/// - Offset 0x0A: unsigned __int16 current (2 bytes)
/// - Offset 0x0C: unsigned __int16 difference (2 bytes)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_PC_NATURAL_RECOVERY_SP
{
	/// <summary>
	/// Object instance ID of the player character
	/// Offset: 0x02 (after PACKET_FIX header)
	/// Type: __int64 (long)
	/// </summary>
	public long objInstID;

	/// <summary>
	/// Current SP value after recovery
	/// Offset: 0x0A
	/// Type: unsigned __int16 (ushort)
	/// </summary>
	public ushort current;

	/// <summary>
	/// SP recovery amount (difference)
	/// Offset: 0x0C
	/// Type: unsigned __int16 (ushort)
	/// Indicates how much SP was recovered in this tick
	/// </summary>
	public ushort difference;
}
