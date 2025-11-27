using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

/// <summary>
/// Party member state information including stats, job, and position.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// Size: 52 bytes
/// Contains all dynamic party member information for UI display.
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PARTY_MEMBER_STATE
{
	/// <summary>Combat job type ID</summary>
	public sbyte combatJobTypeID;

	/// <summary>Life job type ID</summary>
	public sbyte lifeJobTypeID;

	/// <summary>Currently active job (0=combat, 1=life)</summary>
	public byte playingJob;

	/// <summary>Padding for alignment</summary>
	private byte _padding;

	/// <summary>Combat job level</summary>
	public int combatJobLv;

	/// <summary>Life job level</summary>
	public int lifeJobLv;

	/// <summary>Current HP</summary>
	public int hp;

	/// <summary>Maximum HP</summary>
	public int mhp;

	/// <summary>Current MP</summary>
	public int mp;

	/// <summary>Maximum MP</summary>
	public int mmp;

	/// <summary>Current LP (Life Points)</summary>
	public int lp;

	/// <summary>Maximum LP</summary>
	public int mlp;

	/// <summary>Position in world</summary>
	public POS pos;

	/// <summary>Zone ID where member is located</summary>
	public uint zoneID;
}
