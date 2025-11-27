/// <summary>
/// Server->Client packet when a teleporter/portal object enters the player's sight.
/// Transfer objects are portals or teleporters that move players between zones or locations.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_ENTER_SIGHT_TRANSFER
/// Size: 72 bytes total
/// Memory Layout (IDA):
/// - 0x00-0x03: PACKET_VAR header (4 bytes, handled by framework)
/// - 0x04-0x0B: __int64 objInstID (8 bytes) - Teleporter object instance ID
/// - 0x0C-0x2A: char[31] name (31 bytes) - Teleporter name
/// - 0x2B-0x36: FPOS pos (12 bytes) - Current position
/// - 0x37-0x3A: unsigned int targetZoneTypeID (4 bytes) - Destination zone type ID
/// - 0x3B-0x46: FPOS targetPos (12 bytes) - Destination position
/// - 0x47: unsigned __int8 stopType (1 byte) - Movement stop type flag
/// Note: This packet does NOT use PACKET_SC_ENTER_ZONEOBJ unlike other ENTER_SIGHT packets
/// </remarks>
using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_ENTER_SIGHT_TRANSFER : IPacketVariable
{
	/// <summary>Teleporter object instance ID</summary>
	public long objInstID;

	/// <summary>Teleporter name (max 30 chars + null terminator)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 31)]
	public byte[] name;

	/// <summary>Current position of teleporter</summary>
	public FPOS pos;

	/// <summary>Destination zone type ID</summary>
	public uint targetZoneTypeID;

	/// <summary>Destination spawn position</summary>
	public FPOS targetPos;

	/// <summary>Movement stop type flag</summary>
	public byte stopType;
}
