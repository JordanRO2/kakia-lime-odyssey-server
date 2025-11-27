/// <summary>
/// Server packet sent to broadcast that a player has selected a combat job.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_SELECTED_COMBAT_JOB
/// Size: 11 bytes
/// Ordinal: 2863
/// Notifies players about combat job selection (warrior, mage, etc.).
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_SELECTED_COMBAT_JOB
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the player who selected the job</summary>
	public long objInstID;

	/// <summary>Job type ID (combat job category)</summary>
	public byte jobTypeID;
}
