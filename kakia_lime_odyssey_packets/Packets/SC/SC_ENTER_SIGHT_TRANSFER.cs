using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet when a teleporter/portal object enters the player's sight.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_ENTER_SIGHT_TRANSFER
/// Size: 72 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_VAR header (4 bytes) - handled by IPacketVar
/// - 0x04: __int64 objInstID (8 bytes)
/// - 0x0C: char[31] name (31 bytes)
/// - 0x2B: FPOS pos (12 bytes)
/// - 0x37: unsigned int targetZoneTypeID (4 bytes)
/// - 0x3B: FPOS targetPos (12 bytes)
/// - 0x47: unsigned __int8 stopType (1 byte)
/// Note: Does NOT use PACKET_SC_ENTER_ZONEOBJ unlike other ENTER_SIGHT packets
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_ENTER_SIGHT_TRANSFER : IPacketVar
{
	/// <summary>Teleporter object instance ID (offset 0x04)</summary>
	public long objInstID;

	/// <summary>Teleporter name (offset 0x0C)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 31)]
	public byte[] name;

	/// <summary>Current position (offset 0x2B)</summary>
	public FPOS pos;

	/// <summary>Destination zone type ID (offset 0x37)</summary>
	public uint targetZoneTypeID;

	/// <summary>Destination position (offset 0x3B)</summary>
	public FPOS targetPos;

	/// <summary>Movement stop type flag (offset 0x47)</summary>
	public byte stopType;
}
