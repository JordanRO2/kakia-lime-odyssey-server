/// <summary>
/// Server packet sent to broadcast that a player has selected an action target.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_SELECT_ACTION_TARGET
/// Size: 18 bytes
/// Ordinal: 2500
/// Notifies nearby players about target selection.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_SELECT_ACTION_TARGET
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the entity that selected a target</summary>
	public long objInstID;

	/// <summary>Instance ID of the selected target</summary>
	public long targetInstID;
}
