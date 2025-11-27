using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_RESURRECTED @ 14 bytes
/// Sent when an object resurrects at their current location.
/// Notifies client that the object is now alive with specified HP.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_RESURRECTED
{
	/// <summary>Instance ID of the object that resurrected</summary>
	public long objInstID;

	/// <summary>HP amount after resurrection</summary>
	public uint hp;
}
