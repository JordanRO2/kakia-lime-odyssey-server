/// <summary>
/// Client packet requesting to change job class.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_CS_CHANGE_JOB_CLASS
/// Size: 3 bytes
/// Ordinal: 2509
/// Sent when the player wants to change their job class.
/// Server responds with SC_CHANGED_JOB_CLASS.
/// Requires database update to characters table.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.CS;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_CS_CHANGE_JOB_CLASS
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>The job class ID to change to</summary>
	public byte jobClass;
}
