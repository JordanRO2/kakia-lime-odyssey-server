using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_TYPICAL_EXCEPTION - Server exception identified by type ID
/// IDA Structure: PACKET_SC_TYPICAL_EXCEPTION (4 bytes)
/// Verified against IDA Pro: 2025-11-26
///
/// Server sends this when a typical/predefined error occurs, identified by type ID.
/// The client likely has a lookup table of error messages based on typeID.
///
/// Structure layout (from IDA):
/// 0x00: PACKET_FIX (2 bytes) - base packet header
/// 0x02: unsigned __int16 typeID - exception type identifier
/// Total: 4 bytes
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_TYPICAL_EXCEPTION : IPacketFixed
{
    /// <summary>Exception type identifier for predefined error messages</summary>
    public ushort typeID;
}
