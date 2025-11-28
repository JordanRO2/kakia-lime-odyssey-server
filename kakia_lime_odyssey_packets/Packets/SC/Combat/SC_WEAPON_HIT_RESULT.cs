using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Weapon hit result packet sent from server to client
/// Verified against IDA: PACKET_SC_WEAPON_HIT_RESULT (Size: 64 bytes)
/// Date: 2025-11-26
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_WEAPON_HIT_RESULT : IPacketFixed
{
	/// <summary>
	/// Instance ID of the attacker (0x2)
	/// IDA: __int64 fromInstID
	/// </summary>
	public long fromInstID;

	/// <summary>
	/// Instance ID of the target (0xA)
	/// IDA: __int64 targetInstID
	/// </summary>
	public long targetInstID;

	/// <summary>
	/// Whether the target glared/blocked (0x12)
	/// IDA: bool glared
	/// </summary>
	[MarshalAs(UnmanagedType.U1)]
	public bool glared;

	/// <summary>
	/// Animation speed ratio (0x13)
	/// IDA: float aniSpeedRatio
	/// </summary>
	public float aniSpeedRatio;

	/// <summary>
	/// Main hand hit description (0x17)
	/// IDA: HIT_DESC main
	/// </summary>
	public HIT_DESC main;

	/// <summary>
	/// Off-hand/secondary hit description (0x27)
	/// IDA: HIT_DESC sub
	/// </summary>
	public HIT_DESC sub;

	/// <summary>
	/// Whether this is a ranged attack (0x37)
	/// IDA: bool ranged
	/// </summary>
	[MarshalAs(UnmanagedType.U1)]
	public bool ranged;

	/// <summary>
	/// Delay for ranged hit in milliseconds (0x38)
	/// IDA: unsigned int rangeHitDelay
	/// </summary>
	public uint rangeHitDelay;

	/// <summary>
	/// Velocity of ranged projectile (0x3C)
	/// IDA: float rangedVelocity
	/// </summary>
	public float rangedVelocity;
}
