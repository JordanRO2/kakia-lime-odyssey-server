using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.CS;

/// <summary>
/// Client->Server packet to distribute combat job status points.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_CS_DISTRIBUTE_COMBAT_JOB_STATUS_POINT
/// Size: 16 bytes total
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX header (2 bytes) - handled by IPacketFixed
/// - 0x02: unsigned __int16 supplyedSTR (2 bytes)
/// - 0x04: unsigned __int16 supplyedINL (2 bytes)
/// - 0x06: unsigned __int16 supplyedDEX (2 bytes)
/// - 0x08: unsigned __int16 supplyedAGI (2 bytes)
/// - 0x0A: unsigned __int16 supplyedVIT (2 bytes)
/// - 0x0C: unsigned __int16 supplyedSPI (2 bytes)
/// - 0x0E: unsigned __int16 supplyedLUK (2 bytes)
/// Response: SC_DISTRIBUTED_COMBAT_JOB_STATUS_POINT
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CS_DISTRIBUTE_COMBAT_JOB_STATUS_POINT : IPacketFixed
{
	/// <summary>Points to add to STR (offset 0x02)</summary>
	public ushort supplyedSTR;

	/// <summary>Points to add to INL (offset 0x04)</summary>
	public ushort supplyedINL;

	/// <summary>Points to add to DEX (offset 0x06)</summary>
	public ushort supplyedDEX;

	/// <summary>Points to add to AGI (offset 0x08)</summary>
	public ushort supplyedAGI;

	/// <summary>Points to add to VIT (offset 0x0A)</summary>
	public ushort supplyedVIT;

	/// <summary>Points to add to SPI (offset 0x0C)</summary>
	public ushort supplyedSPI;

	/// <summary>Points to add to LUK (offset 0x0E)</summary>
	public ushort supplyedLUK;
}
