using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet indicating crafting has finished.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_STUFF_MAKE_FINISH
/// Size: 15 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: unsigned __int8 finishResult (1 byte)
/// - 0x03: __int64 InstID (8 bytes)
/// - 0x0B: unsigned __int16 useLP (2 bytes)
/// - 0x0D: unsigned __int16 currentLP (2 bytes)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_STUFF_MAKE_FINISH : IPacketFixed
{
	/// <summary>Crafting result (offset 0x02)</summary>
	public byte finishResult;

	/// <summary>Crafted item instance ID (offset 0x03)</summary>
	public long InstID;

	/// <summary>LP used (offset 0x0B)</summary>
	public ushort useLP;

	/// <summary>Current LP remaining (offset 0x0D)</summary>
	public ushort currentLP;
}
