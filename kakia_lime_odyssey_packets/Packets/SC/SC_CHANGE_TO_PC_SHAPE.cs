using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet to change an object's appearance to PC shape.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_CHANGE_TO_PC_SHAPE
/// Size: 162 bytes
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes)
/// - 0x02: __int64 objInstID (8 bytes) - Object instance ID to transform
/// - 0x0A: APPEARANCE_PC apperance (152 bytes) - PC appearance data
/// Used when an object needs to transform into a PC appearance (shapeshifting, polymorphing).
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_CHANGE_TO_PC_SHAPE : IPacketFixed
{
	public long objInstID;
	public APPEARANCE_PC_KR apperance;
}
