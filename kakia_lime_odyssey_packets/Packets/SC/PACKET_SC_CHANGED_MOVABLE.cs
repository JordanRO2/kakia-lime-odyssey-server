/// <summary>
/// Server packet sent when movable status changes.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_CHANGED_MOVABLE
/// Size: 18 bytes
/// Ordinal: 21043
/// Controls whether the entity can move (e.g., stunned, rooted, etc.).
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_CHANGED_MOVABLE : IPacketFixed
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the object</summary>
	public long objInstID;

	/// <summary>Current movable value</summary>
	public int current;

	/// <summary>Movable change amount (delta)</summary>
	public int update;

	public const int Size = 18;
}
