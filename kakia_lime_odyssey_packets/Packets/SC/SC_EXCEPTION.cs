using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_EXCEPTION - Server exception/error message with description
/// IDA Structure: PACKET_SC_EXCEPTION (516 bytes)
/// Verified against IDA Pro: 2025-11-26
///
/// Server sends this when an error or exception occurs, providing a detailed description.
/// Used for debugging and error reporting to the client.
///
/// Structure layout (from IDA):
/// 0x00: PACKET_FIX (2 bytes) - base packet header
/// 0x02: char[513] desc - exception description string
/// 0x203: bool isImportant - whether this is a critical exception
/// Total: 516 bytes
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_EXCEPTION : IPacketFixed
{
    /// <summary>Exception description message (513 bytes, null-terminated string)</summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 513)]
    public byte[] desc;

    /// <summary>Whether this is a critical/important exception</summary>
    public bool isImportant;
}
