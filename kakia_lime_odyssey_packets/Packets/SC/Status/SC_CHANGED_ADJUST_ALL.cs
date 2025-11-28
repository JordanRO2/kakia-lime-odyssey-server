/// <summary>
/// SC_ versions of CHANGED_ADJUST packets without PACKET_FIX header.
/// These packets modify skill/item casting parameters.
/// </summary>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Interface;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_CHANGED_ADJUST_SKILL_CASTING (18 bytes total)
/// Affects the casting time for skills.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_CHANGED_ADJUST_SKILL_CASTING : IPacketFixed
{
	/// <summary>Instance ID of the object</summary>
	public long objInstID;

	/// <summary>Current skill casting adjustment value</summary>
	public int current;

	/// <summary>Skill casting adjustment change amount (delta)</summary>
	public int update;
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_CHANGED_ADJUST_SKILL_CASTING_RATIO (18 bytes total)
/// Affects the casting time ratio for skills.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_CHANGED_ADJUST_SKILL_CASTING_RATIO : IPacketFixed
{
	/// <summary>Instance ID of the object</summary>
	public long objInstID;

	/// <summary>Current skill casting ratio value</summary>
	public int current;

	/// <summary>Skill casting ratio change amount (delta)</summary>
	public int update;
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_CHANGED_ADJUST_SKILL_APPLYING_RANGE (18 bytes total)
/// Affects the applying range for skills.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_CHANGED_ADJUST_SKILL_APPLYING_RANGE : IPacketFixed
{
	/// <summary>Instance ID of the object</summary>
	public long objInstID;

	/// <summary>Current skill applying range value</summary>
	public int current;

	/// <summary>Skill applying range change amount (delta)</summary>
	public int update;
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_CHANGED_ADJUST_SKILL_USE_MP (18 bytes total)
/// Affects the MP usage for skills.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_CHANGED_ADJUST_SKILL_USE_MP : IPacketFixed
{
	/// <summary>Instance ID of the object</summary>
	public long objInstID;

	/// <summary>Current skill MP usage adjustment value</summary>
	public int current;

	/// <summary>Skill MP usage adjustment change amount (delta)</summary>
	public int update;
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_CHANGED_ADJUST_ITEM_CASTING (18 bytes total)
/// Affects the casting time for items.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_CHANGED_ADJUST_ITEM_CASTING : IPacketFixed
{
	/// <summary>Instance ID of the object</summary>
	public long objInstID;

	/// <summary>Current item casting adjustment value</summary>
	public int current;

	/// <summary>Item casting adjustment change amount (delta)</summary>
	public int update;
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_CHANGED_ADJUST_ITEM_CASTING_RATIO (18 bytes total)
/// Affects the casting time ratio for items.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_CHANGED_ADJUST_ITEM_CASTING_RATIO : IPacketFixed
{
	/// <summary>Instance ID of the object</summary>
	public long objInstID;

	/// <summary>Current item casting ratio value</summary>
	public int current;

	/// <summary>Item casting ratio change amount (delta)</summary>
	public int update;
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_CHANGED_ADJUST_ITEM_APPLYING_RANGE (18 bytes total)
/// Affects the applying range for items.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_CHANGED_ADJUST_ITEM_APPLYING_RANGE : IPacketFixed
{
	/// <summary>Instance ID of the object</summary>
	public long objInstID;

	/// <summary>Current item applying range value</summary>
	public int current;

	/// <summary>Item applying range change amount (delta)</summary>
	public int update;
}
