using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

/// <summary>
/// SKILL - Player skill data structure
/// IDA Verification: 2025-11-26
/// IDA Address: Ordinal 14626
///
/// IDA Structure:
/// - typeID: unsigned int (4 bytes) at offset 0x00 - Skill type identifier
/// - level: unsigned short (2 bytes) at offset 0x04 - Current skill level
/// - remainCoolTime: unsigned int (4 bytes) at offset 0x08 - Remaining cooldown time in milliseconds
/// Total size: 12 bytes
///
/// Verified: All fields match IDA structure exactly
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct SKILL
{
	public uint typeID;
	public ushort level;
	public uint remainCoolTime;
}
