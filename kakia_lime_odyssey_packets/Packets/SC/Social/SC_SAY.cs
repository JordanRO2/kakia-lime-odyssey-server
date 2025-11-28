using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client local chat message broadcast.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_SAY
/// Size: 20 bytes total (variable-length packet)
/// Memory Layout (IDA):
/// - 0x00: PACKET_VAR header (4 bytes) - handled by IPacketVar
/// - 0x04: __int64 objInstID (8 bytes)
/// - 0x0C: unsigned int maintainTime (4 bytes)
/// - 0x10: int type (4 bytes)
/// Note: Variable-length message string follows
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_SAY : IPacketVar
{
	/// <summary>Sender's object instance ID (offset 0x04)</summary>
	public long objInstID;

	/// <summary>Message display duration (offset 0x0C)</summary>
	public uint maintainTime;

	/// <summary>Message type/flags (offset 0x10)</summary>
	public int type;

	/// <summary>
	/// Chat message text (not part of struct layout, handled separately)
	/// This field is not marshaled - it's for C# convenience only
	/// </summary>
	[field: NonSerialized]
	public string message { get; set; }
}
