/// <summary>
/// Server packet sent when an entity's LP (Life Points) changes.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_CHANGED_LP
/// Size: 18 bytes
/// Ordinal: 17246
/// LP is used for life job activities (gathering, crafting, etc.).
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_CHANGED_LP
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the object whose LP changed</summary>
	public long objInstID;

	/// <summary>Current LP value</summary>
	public int current;

	/// <summary>LP change amount (delta)</summary>
	public int update;
}
