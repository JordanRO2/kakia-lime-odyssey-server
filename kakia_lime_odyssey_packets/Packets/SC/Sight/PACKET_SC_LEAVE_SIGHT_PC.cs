/// <summary>
/// Server packet sent when a player character leaves the client's sight range.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_LEAVE_SIGHT_PC
/// Size: 10 bytes
/// Ordinal: 2533
/// Inherits from PACKET_SC_LEAVE_ZONEOBJ.
/// Notifies the client to remove a player character from their visible entities.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Interface;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_LEAVE_SIGHT_PC : IPacketFixed
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the player character leaving sight range</summary>
	public long objInstID;
}
