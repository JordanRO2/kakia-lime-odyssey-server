/// <summary>
/// Server packet sent with detailed crafting report information.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_ITEM_MAKE_UPDATE_REPORT
/// Size: 652 bytes
/// Ordinal: 2625
/// Contains success rates, materials needed, result items, and critical craft info.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_ITEM_MAKE_UPDATE_REPORT
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Recipe/item type ID</summary>
	public uint typeID;

	/// <summary>Base success percentage (0-100)</summary>
	public int successPercent;

	/// <summary>Critical success percentage (0-100)</summary>
	public int criticalSuccessPercent;

	/// <summary>Time required to craft in milliseconds</summary>
	public uint makeTime;

	/// <summary>LP (Life Points) required for crafting</summary>
	public ushort requestLP;

	/// <summary>Normal result item type ID</summary>
	public int itemTypeID;

	/// <summary>Normal result item count</summary>
	public uint count;

	/// <summary>Normal result item durability</summary>
	public int durability;

	/// <summary>Normal result item max durability</summary>
	public int mdurability;

	/// <summary>Normal result item inherit properties</summary>
	public ITEM_INHERITS inherits;

	/// <summary>Critical result item type ID</summary>
	public int criticalItemTypeID;

	/// <summary>Critical result item count</summary>
	public uint criticalCount;

	/// <summary>Critical result item durability</summary>
	public int criticalDurability;

	/// <summary>Critical result item max durability</summary>
	public int criticalMdurability;

	/// <summary>Critical result item inherit properties</summary>
	public ITEM_INHERITS criticalInherits;
}
