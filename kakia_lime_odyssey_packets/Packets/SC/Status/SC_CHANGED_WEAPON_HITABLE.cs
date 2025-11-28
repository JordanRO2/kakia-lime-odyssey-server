using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Weapon hitable state change packet sent from server to client
/// Indicates whether an entity can be hit with weapon attacks
/// Verified against IDA: PACKET_SC_CHANGED_WEAPON_HITABLE (Size: 18 bytes)
/// Date: 2025-11-26
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_CHANGED_WEAPON_HITABLE : IPacketFixed
{
	/// <summary>
	/// Instance ID of the object (0x2)
	/// IDA: __int64 objInstID
	/// </summary>
	public long objInstID;

	/// <summary>
	/// Current hitable value (0xA)
	/// IDA: int current
	/// </summary>
	public int current;

	/// <summary>
	/// Updated hitable value (0xE)
	/// IDA: int update
	/// </summary>
	public int update;
}
