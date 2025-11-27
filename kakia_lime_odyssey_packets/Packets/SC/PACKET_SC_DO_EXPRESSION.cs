/// <summary>
/// Server packet instructing a client to play a facial expression animation on an object.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_DO_EXPRESSION
/// Size: 11 bytes
/// Ordinal: 2853
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX (2 bytes) - Packet header
/// - 0x02: __int64 objInstID (8 bytes) - Instance ID of object performing expression
/// - 0x0A: unsigned __int8 expressionType (1 byte) - Expression type ID
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_DO_EXPRESSION
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the object performing the expression</summary>
	public long objInstID;

	/// <summary>Expression type ID (smile, frown, etc.)</summary>
	public byte expressionType;
}
