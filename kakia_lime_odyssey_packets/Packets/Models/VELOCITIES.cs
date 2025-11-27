/// <summary>
/// Movement velocity structure containing all movement speeds and acceleration values.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: VELOCITIES
/// Size: 44 bytes
/// Contains speeds for run, walk, swim (forward and backward) with acceleration values.
/// </remarks>
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct VELOCITIES
{
	/// <summary>Overall velocity ratio multiplier</summary>
	public float ratio;

	/// <summary>Running speed</summary>
	public float run;

	/// <summary>Running acceleration</summary>
	public float runAccel;

	/// <summary>Walking speed</summary>
	public float walk;

	/// <summary>Walking acceleration</summary>
	public float walkAccel;

	/// <summary>Backward walking speed</summary>
	public float backwalk;

	/// <summary>Backward walking acceleration</summary>
	public float backwalkAccel;

	/// <summary>Swimming speed</summary>
	public float swim;

	/// <summary>Swimming acceleration</summary>
	public float swimAccel;

	/// <summary>Backward swimming speed</summary>
	public float backSwim;

	/// <summary>Backward swimming acceleration</summary>
	public float backSwimAccel;
}
