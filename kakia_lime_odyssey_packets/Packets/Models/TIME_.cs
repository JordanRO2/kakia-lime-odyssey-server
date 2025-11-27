using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: TIME_ (3 bytes)
/// Memory Layout:
///   - unsigned __int8 hour (1 byte, offset 0x00)
///   - unsigned __int8 minute (1 byte, offset 0x01)
///   - unsigned __int8 second (1 byte, offset 0x02)
/// Purpose: Represents game time (hour, minute, second)
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TIME_
{
	public byte hour;
	public byte minute;
	public byte second;
}
