using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

/// <summary>
/// Base structure for sight extension data used in appearance change packets.
/// Contains a single byte indicating the type of appearance change.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: SIGHT_EXTEND
/// Size: 1 byte
/// Memory Layout (IDA):
/// - 0x00: unsigned __int8 type (1 byte) - Type of sight extension/appearance change
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SIGHT_EXTEND
{
	public byte type;
}
