using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet to prepare for material gathering/processing.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_STUFF_MAKE_READY
/// Size: 6 bytes total (2-byte header + 4-byte payload)
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: unsigned int typeID (4 bytes)
/// Response: SC_STUFF_MAKE_READY_SUCCESS
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_STUFF_MAKE_READY : IPacketFixed
{
	/// <summary>Type ID of the gathering/processing action</summary>
	public uint typeID;
}
