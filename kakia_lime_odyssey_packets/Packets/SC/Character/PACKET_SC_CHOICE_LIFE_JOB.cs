/// <summary>
/// Server packet prompting the player to choose a life job.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_CHOICE_LIFE_JOB
/// Size: 12 bytes
/// Ordinal: 2845
/// Opens the life job selection dialog. Contains variable data for job options.
/// Note: This is a PACKET_VAR (variable length) packet.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_CHOICE_LIFE_JOB
{
	/// <summary>Packet header (variable length)</summary>
	public PACKET_VAR header;

	/// <summary>Instance ID of NPC or trigger object</summary>
	public long objInstID;
}
