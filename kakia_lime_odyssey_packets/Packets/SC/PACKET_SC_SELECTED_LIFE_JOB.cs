/// <summary>
/// Server packet sent to broadcast that a player has selected a life job.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_SELECTED_LIFE_JOB
/// Size: 11 bytes
/// Ordinal: 2847
/// Notifies players about life job selection (gathering, crafting, etc.).
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_SELECTED_LIFE_JOB
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Instance ID of the player who selected the job</summary>
	public long objInstID;

	/// <summary>Job type ID (life job category) - IDA shows 'char' but byte is equivalent in C#</summary>
	public sbyte jobTypeID;
}
