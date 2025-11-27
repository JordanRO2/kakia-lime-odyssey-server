using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

/// <summary>
/// Guild member state information
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: GUILD_MEMBER_STATE
/// Size: 20 bytes
/// Note: 2 bytes padding after lifeJobTypeID (offset 0x01->0x04)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct GUILD_MEMBER_STATE
{
	/// <summary>Combat job type ID</summary>
	public byte combatJobTypeID;

	/// <summary>Life job type ID</summary>
	public byte lifeJobTypeID;

	/// <summary>Padding to align to 4-byte boundary</summary>
	private ushort padding;

	/// <summary>Combat job level</summary>
	public int combatJobLv;

	/// <summary>Life job level</summary>
	public int lifeJobLv;

	/// <summary>Guild contribution points</summary>
	public int point;

	/// <summary>Member type (rank/role in guild)</summary>
	public int memberType;
}
