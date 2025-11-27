using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Base structure for zone object entry, used when objects enter the player's view range.
/// Contains object instance ID and position/direction data.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_ENTER_ZONEOBJ
/// Size: 36 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_VAR header (ushort header, ushort size) - 4 bytes (handled by framework, not in struct)
/// - 0x04: __int64 objInstID - 8 bytes
/// - 0x0C: FPOS pos (x,y,z floats) - 12 bytes
/// - 0x18: FPOS dir (x,y,z floats) - 12 bytes
/// Note: The PACKET_VAR header (4 bytes) is embedded at the start but handled by the serialization
/// framework, so the C# struct omits it. The struct fields start at offset 0x04 in IDA.
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct SC_ENTER_ZONEOBJ
{
	public long objInstID;
	public FPOS pos;
	public FPOS dir;
}
