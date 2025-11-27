/// <summary>
/// Server packet sent when an item projectile leaves the client's sight range.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_LEAVE_SIGHT_BULLET_ITEM
/// Size: 10 bytes
/// Ordinal: 2551
/// Inherits from PACKET_SC_LEAVE_ZONEOBJ.
/// Notifies the client to remove an item bullet/projectile from their visible entities.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Interface;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_LEAVE_SIGHT_BULLET_ITEM : IPacketFixed
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the item bullet leaving sight range</summary>
	public long objInstID;
}
