using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Interface;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet to trigger an expression/emote on an object.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_DO_EXPRESSION
/// Size: 11 bytes
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes)
/// - 0x02: __int64 objInstID (8 bytes) - Object performing the expression
/// - 0x0A: unsigned __int8 expressionType (1 byte) - Expression/emote type ID
/// Used to display character expressions/emotes (happy, sad, wave, etc).
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_DO_EXPRESSION : IPacketFixed
{
	public long objInstID;
	public byte expressionType;
}
