/// <summary>
/// Server packet prompting player to choose a combat job.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_CHOICE_COMBAT_JOB
/// Size: 12 bytes
/// Ordinal: 2861
/// Sent when player reaches a level or point where they can select a combat job (warrior, mage, etc.).
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_CHOICE_COMBAT_JOB
{
	/// <summary>Packet header (PACKET_VAR - 4 bytes)</summary>
	public uint header;

	/// <summary>Instance ID of the player object being prompted</summary>
	public long objInstID;
}
