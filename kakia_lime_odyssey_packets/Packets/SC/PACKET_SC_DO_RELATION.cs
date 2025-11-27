/// <summary>
/// Server packet updating the race/faction relationship state of an object.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_DO_RELATION
/// Size: 14 bytes
/// Ordinal: 2854
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX (2 bytes) - Packet header
/// - 0x02: __int64 objInstID (8 bytes) - Instance ID of object
/// - 0x0A: int raceRelationState (4 bytes) - Relationship state (friendly, hostile, neutral, etc.)
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_DO_RELATION
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the object</summary>
	public long objInstID;

	/// <summary>Race/faction relationship state</summary>
	public int raceRelationState;
}
