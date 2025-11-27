using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

/// <summary>
/// STATUS_PC - Player character status structure
/// IDA verified: 2025-11-26
/// Structure name in client: STATUS_PC
/// Total size: 176 bytes
/// </summary>
/// <remarks>
/// Contains all player status information including combat stats, job info, and velocities.
/// Used in PACKET_SC_PC_STATUS and other status-related packets.
/// IDA Details:
/// - Size: 176 bytes
/// - Member count: 21
/// - Offset 0x00: COMMON_STATUS (20 bytes)
/// - Offset 0x14: lp (4 bytes)
/// - Offset 0x18: mlp (4 bytes)
/// - Offset 0x1C: streamPoint (4 bytes)
/// - Offset 0x20: meleeHitRate (4 bytes float)
/// - Offset 0x24: dodge (4 bytes float)
/// - Offset 0x28: meleeAtk (4 bytes)
/// - Offset 0x2C: meleeDefense (4 bytes)
/// - Offset 0x30: spellAtk (4 bytes)
/// - Offset 0x34: spellDefense (4 bytes)
/// - Offset 0x38: parry (4 bytes float)
/// - Offset 0x3C: block (4 bytes float)
/// - Offset 0x40: resist (4 bytes)
/// - Offset 0x44: criticalRate (4 bytes float)
/// - Offset 0x48: hitSpeedRatio (4 bytes float)
/// - Offset 0x4C: lifeJob (20 bytes LIFE_JOB_STATUS_)
/// - Offset 0x60: combatJob (24 bytes COMBAT_JOB_STATUS_)
/// - Offset 0x78: velocities (44 bytes VELOCITIES)
/// - Offset 0xA4: collectSucessRate (4 bytes float)
/// - Offset 0xA8: collectionIncreaseRate (4 bytes float)
/// - Offset 0xAC: makeTimeDecreaseAmount (4 bytes float)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct STATUS_PC
{
	public COMMON_STATUS commonStatus;
	public uint lp;
	public uint mlp;
	public uint streamPoint;
	public float meleeHitRate;
	public float dodge;
	public uint meleeAtk;
	public uint meleeDefense;
	public uint spellAtk;
	public uint spellDefense;
	public float parry;
	public float block;
	public uint resist;
	public float criticalRate;
	public float hitSpeedRatio;
	public LIFE_JOB_STATUS_ lifeJob;
	public COMBAT_JOB_STATUS_ combatJob;
	public VELOCITIES velocities;
	public float collectSucessRate;
	public float collectionIncreaseRate;
	public float makeTimeDecreaseAmount;
}
