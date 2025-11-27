using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server notifies client about player character appearance update.
/// Sent when player changes equipment, performs cosmetic changes, or updates visual appearance.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_UPDATED_APPEARANCE_PC
/// Size: 162 bytes (2 header + 8 objInstID + 152 appearance)
/// Packet Type: PACKET_FIX (fixed length header, 2 bytes)
/// Memory Layout (IDA):
/// - 0x00: PACKET_FIX (base header) - 2 bytes
/// - 0x02: __int64 objInstID - 8 bytes (object instance ID)
/// - 0x0A: APPEARANCE_PC apperance - 152 bytes (full appearance data)
/// Note: The field name in IDA is "apperance" (typo, missing second 'a').
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_UPDATED_APPEARANCE_PC : IPacketFixed
{
	public ulong objInstID;
	public APPEARANCE_PC_KR appearance;
}
