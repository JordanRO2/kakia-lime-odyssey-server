using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet sent when a character levels up their combat job.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_PC_COMBAT_JOB_LEVEL_UP
/// Size: 27 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: __int64 objInstID (8 bytes)
/// - 0x0A: unsigned __int8 lv (1 byte)
/// - 0x0B: unsigned int exp (4 bytes)
/// - 0x0F: unsigned __int16 newStr (2 bytes)
/// - 0x11: unsigned __int16 newAgi (2 bytes)
/// - 0x13: unsigned __int16 newVit (2 bytes)
/// - 0x15: unsigned __int16 newInl (2 bytes)
/// - 0x17: unsigned __int16 newDex (2 bytes)
/// - 0x19: unsigned __int16 newSpi (2 bytes)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_PC_COMBAT_JOB_LEVEL_UP : IPacketFixed
{
	/// <summary>Instance ID of the character leveling up (offset 0x02)</summary>
	public long objInstID;

	/// <summary>New combat job level (offset 0x0A)</summary>
	public byte lv;

	/// <summary>Current experience after level up (offset 0x0B)</summary>
	public uint exp;

	/// <summary>New Strength stat value (offset 0x0F)</summary>
	public ushort newStr;

	/// <summary>New Agility stat value (offset 0x11)</summary>
	public ushort newAgi;

	/// <summary>New Vitality stat value (offset 0x13)</summary>
	public ushort newVit;

	/// <summary>New Intelligence stat value (offset 0x15)</summary>
	public ushort newInl;

	/// <summary>New Dexterity stat value (offset 0x17)</summary>
	public ushort newDex;

	/// <summary>New Spirit stat value (offset 0x19)</summary>
	public ushort newSpi;
}
