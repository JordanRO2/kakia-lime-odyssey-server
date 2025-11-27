/// <summary>
/// Life job status information structure.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: LIFE_JOB_STATUS_
/// Size: 20 bytes
/// Note: 3 byte padding between lv and exp fields
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Common;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
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
