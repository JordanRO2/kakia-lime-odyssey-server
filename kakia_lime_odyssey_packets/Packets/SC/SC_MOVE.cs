using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_MOVE - Server broadcasts player movement to clients
/// Sent when a player character moves in the world
/// Total size: 59 bytes (2 byte header handled by IPacketFixed + 57 byte payload)
/// IDA verified: 2025-11-27
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_MOVE : IPacketFixed
{
	/// <summary>Object instance ID of the moving character (8 bytes)</summary>
	public long objInstID;

	/// <summary>Current position (12 bytes: x, y, z floats)</summary>
	public FPOS pos;

	/// <summary>Movement direction vector (12 bytes: x, y, z floats)</summary>
	public FPOS dir;

	/// <summary>Delta look-at rotation in radians (4 bytes)</summary>
	public float deltaLookAtRadian;

	/// <summary>Server tick timestamp (4 bytes)</summary>
	public uint tick;

	/// <summary>Movement velocity (4 bytes)</summary>
	public float velocity;

	/// <summary>Acceleration (4 bytes)</summary>
	public float accel;

	/// <summary>Turning speed (4 bytes)</summary>
	public float turningSpeed;

	/// <summary>Velocity ratio/multiplier (4 bytes)</summary>
	public float velocityRatio;

	/// <summary>Movement type identifier (1 byte: walk, run, etc.)</summary>
	public byte moveType;
}
