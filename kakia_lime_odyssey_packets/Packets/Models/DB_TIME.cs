using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

/// <summary>
/// Database timestamp structure
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: DB_TIME
/// Size: 16 bytes
/// Used in: GUILD and various time-related packets
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct DB_TIME
{
	/// <summary>Year</summary>
	public short year;

	/// <summary>Month (1-12)</summary>
	public ushort month;

	/// <summary>Day (1-31)</summary>
	public ushort day;

	/// <summary>Hour (0-23)</summary>
	public ushort hour;

	/// <summary>Minute (0-59)</summary>
	public ushort minute;

	/// <summary>Second (0-59)</summary>
	public ushort second;

	/// <summary>Millisecond (0-999)</summary>
	public uint milliSecond;
}
