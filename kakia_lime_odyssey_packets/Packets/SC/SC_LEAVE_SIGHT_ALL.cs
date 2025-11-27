using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_LEAVE_SIGHT_PC (10 bytes)
/// Inherits from: PACKET_SC_LEAVE_ZONEOBJ
/// Purpose: Notifies client when a player character leaves visible range
/// Memory Layout (IDA):
///   - PACKET_FIX header (2 bytes) - handled by IPacketFixed interface
///   - __int64 objInstID (8 bytes) - from PACKET_SC_LEAVE_ZONEOBJ
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct SC_LEAVE_SIGHT_PC : IPacketFixed
{
	public SC_LEAVE_ZONEOBJ leave_zone;
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_LEAVE_SIGHT_MONSTER (10 bytes)
/// Purpose: Notifies client when a monster leaves visible range
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct SC_LEAVE_SIGHT_MONSTER : IPacketFixed
{
	public SC_LEAVE_ZONEOBJ leave_zone;
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_LEAVE_SIGHT_VILLAGER (10 bytes)
/// Purpose: Notifies client when a villager/NPC leaves visible range
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct SC_LEAVE_SIGHT_VILLAGER : IPacketFixed
{
	public SC_LEAVE_ZONEOBJ leave_zone;
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_LEAVE_SIGHT_QUEST_BOARD (10 bytes)
/// Purpose: Notifies client when a quest board leaves visible range
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct SC_LEAVE_SIGHT_QUEST_BOARD : IPacketFixed
{
	public SC_LEAVE_ZONEOBJ leave_zone;
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_LEAVE_SIGHT_PROP (10 bytes)
/// Purpose: Notifies client when a prop object leaves visible range
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct SC_LEAVE_SIGHT_PROP : IPacketFixed
{
	public SC_LEAVE_ZONEOBJ leave_zone;
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_LEAVE_SIGHT_MERCHANT (10 bytes)
/// Purpose: Notifies client when a merchant leaves visible range
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct SC_LEAVE_SIGHT_MERCHANT : IPacketFixed
{
	public SC_LEAVE_ZONEOBJ leave_zone;
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_LEAVE_SIGHT_TRANSFER (10 bytes)
/// Purpose: Notifies client when a teleporter/transfer point leaves visible range
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct SC_LEAVE_SIGHT_TRANSFER : IPacketFixed
{
	public SC_LEAVE_ZONEOBJ leave_zone;
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_LEAVE_SIGHT_HOUSE (10 bytes)
/// Purpose: Notifies client when a house leaves visible range
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct SC_LEAVE_SIGHT_HOUSE : IPacketFixed
{
	public SC_LEAVE_ZONEOBJ leave_zone;
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_LEAVE_SIGHT_BULLET_SKILL (10 bytes)
/// Purpose: Notifies client when a skill projectile leaves visible range
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct SC_LEAVE_SIGHT_BULLET_SKILL : IPacketFixed
{
	public SC_LEAVE_ZONEOBJ leave_zone;
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_LEAVE_SIGHT_BULLET_ITEM (10 bytes)
/// Purpose: Notifies client when an item projectile leaves visible range
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct SC_LEAVE_SIGHT_BULLET_ITEM : IPacketFixed
{
	public SC_LEAVE_ZONEOBJ leave_zone;
}