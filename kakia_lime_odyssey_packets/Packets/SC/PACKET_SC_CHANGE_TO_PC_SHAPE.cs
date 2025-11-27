/// <summary>
/// Server packet transforming an object's appearance to a player character model.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_CHANGE_TO_PC_SHAPE
/// Size: 162 bytes
/// Ordinal: 2851
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX (2 bytes) - Packet header
/// - 0x02: __int64 objInstID (8 bytes) - Instance ID of object to transform
/// - 0x0A: APPEARANCE_PC apperance (152 bytes) - Complete player appearance data
/// Note: Field name is "apperance" (typo) in IDA, matching client implementation.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_CHANGE_TO_PC_SHAPE
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the object to transform</summary>
	public long objInstID;

	/// <summary>Complete player character appearance data to transform into</summary>
	public APPEARANCE_PC_KR apperance;
}
