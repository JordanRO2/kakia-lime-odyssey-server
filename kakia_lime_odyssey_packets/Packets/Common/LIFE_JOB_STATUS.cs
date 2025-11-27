using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Common;

/// <summary>
/// Life job status information structure.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: LIFE_JOB_STATUS_
/// Size: 20 bytes
/// Memory Layout (IDA):
/// - 0x00: unsigned __int8 lv (1 byte)
/// - 0x01-0x03: padding (3 bytes)
/// - 0x04: unsigned int exp (4 bytes)
/// - 0x08: unsigned __int16 statusPoint (2 bytes)
/// - 0x0A: unsigned __int16 idea (2 bytes)
/// - 0x0C: unsigned __int16 mind (2 bytes)
/// - 0x0E: unsigned __int16 craft (2 bytes)
/// - 0x10: unsigned __int16 sense (2 bytes)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct LIFE_JOB_STATUS
{
	/// <summary>Life job level</summary>
	public byte lv;

	/// <summary>Padding bytes for alignment</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	private byte[] padding1;

	/// <summary>Current experience points</summary>
	public uint exp;

	/// <summary>Available status points to distribute</summary>
	public ushort statusPoint;

	/// <summary>Idea stat value</summary>
	public ushort idea;

	/// <summary>Mind stat value</summary>
	public ushort mind;

	/// <summary>Craft stat value</summary>
	public ushort craft;

	/// <summary>Sense stat value</summary>
	public ushort sense;
}
