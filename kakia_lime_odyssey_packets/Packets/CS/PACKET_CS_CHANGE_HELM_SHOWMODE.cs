/// <summary>
/// Client packet requesting to toggle helmet visibility.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_CHANGE_HELM_SHOWMODE
/// Size: 3 bytes
/// Ordinal: 2514
/// Sent when the player toggles helmet visibility in the UI.
/// Server responds with SC_CHANGE_HELM_SHOWMODE.
/// Requires database update to characters table for persistence.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_CHANGE_HELM_SHOWMODE
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>True to show helmet, false to hide it</summary>
	public bool show;
}
