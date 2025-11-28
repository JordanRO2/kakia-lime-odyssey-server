using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet for launching a bullet item at an object.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_LAUNCH_BULLET_ITEM_OBJ
/// Size: 50 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: __int64 fromInstID (8 bytes)
/// - 0x0A: __int64 toInstID (8 bytes)
/// - 0x12: int typeID (4 bytes)
/// - 0x16: unsigned __int16 useSP (2 bytes)
/// - 0x18: unsigned __int16 useHP (2 bytes)
/// - 0x1A: unsigned __int16 useMP (2 bytes)
/// - 0x1C: unsigned __int16 useLP (2 bytes)
/// - 0x1E: unsigned int coolTime (4 bytes)
/// - 0x22: __int64 bulletID (8 bytes)
/// - 0x2A: unsigned int tick (4 bytes)
/// - 0x2E: float velocity (4 bytes)
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_LAUNCH_BULLET_ITEM_OBJ : IPacketFixed
{
	/// <summary>Caster instance ID (offset 0x02)</summary>
	public long fromInstID;

	/// <summary>Target instance ID (offset 0x0A)</summary>
	public long toInstID;

	/// <summary>Item type ID (offset 0x12)</summary>
	public int typeID;

	/// <summary>SP cost (offset 0x16)</summary>
	public ushort useSP;

	/// <summary>HP cost (offset 0x18)</summary>
	public ushort useHP;

	/// <summary>MP cost (offset 0x1A)</summary>
	public ushort useMP;

	/// <summary>LP cost (offset 0x1C)</summary>
	public ushort useLP;

	/// <summary>Cooldown time (offset 0x1E)</summary>
	public uint coolTime;

	/// <summary>Bullet instance ID (offset 0x22)</summary>
	public long bulletID;

	/// <summary>Launch tick (offset 0x2A)</summary>
	public uint tick;

	/// <summary>Bullet velocity (offset 0x2E)</summary>
	public float velocity;
}
