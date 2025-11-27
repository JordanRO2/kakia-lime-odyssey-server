using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// SC_SKILL_LIST - Server sends player's skill list
/// IDA Verification: 2025-11-26
/// Structure: PACKET_VAR (header + size) with variable-length SKILL array
/// IDA Address: Ordinal 2718
///
/// IDA Structure (PACKET_SC_SKILL_LIST):
/// - Size: 4 bytes (base)
/// - Contains: PACKET_VAR (2 byte header + 2 byte size)
/// - Variable data: Array of SKILL structs (12 bytes each)
///
/// PACKET_VAR fields:
/// - header: unsigned short (2 bytes) at offset 0x00
/// - size: unsigned short (2 bytes) at offset 0x02
///
/// SKILL struct (defined in Models/SKILL.cs):
/// - typeID: unsigned int (4 bytes) at offset 0x00
/// - level: unsigned short (2 bytes) at offset 0x04
/// - remainCoolTime: unsigned int (4 bytes) at offset 0x08
/// Total SKILL size: 12 bytes
/// </summary>
public struct SC_SKILL_LIST
{
	public List<SKILL> skills;
}
