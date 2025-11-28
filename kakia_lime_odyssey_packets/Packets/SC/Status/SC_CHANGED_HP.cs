using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Notifies client that an entity's HP has changed.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_CHANGED_HP
/// Size: 26 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: __int64 objInstID (8 bytes) - Instance ID of affected entity
/// - 0x0A: int current (4 bytes) - Current HP value after change
/// - 0x0E: int update (4 bytes) - HP change amount (+heal/-damage)
/// - 0x12: __int64 fromID (8 bytes) - Instance ID of source (attacker/healer)
/// Inherits: PACKET_CHANGED_STATUS base structure
/// Triggered by: Damage, healing, HP drain effects
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_CHANGED_HP : IPacketFixed
{
	/// <summary>Instance ID of the entity whose HP changed (offset 0x02)</summary>
	public long objInstID;

	/// <summary>Current HP value after change (offset 0x0A)</summary>
	public int current;

	/// <summary>HP change amount - positive for heal, negative for damage (offset 0x0E)</summary>
	public int update;

	/// <summary>Instance ID of the entity that caused the change (offset 0x12)</summary>
	public long fromID;
}
