/// <summary>
/// Server packet sent when player gains life job experience.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_GOT_LIFE_JOB_EXP
/// Size: 10 bytes
/// Ordinal: 2606
/// Sent after completing life job activities (crafting, gathering, etc.).
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_GOT_LIFE_JOB_EXP
{
	/// <summary>Packet header</summary>
	public PACKET_FIX header;

	/// <summary>Total experience after the gain</summary>
	public uint exp;

	/// <summary>Amount of experience gained</summary>
	public uint addExp;
}
