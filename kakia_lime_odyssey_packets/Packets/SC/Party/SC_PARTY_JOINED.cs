using kakia_lime_odyssey_packets.Packets.Interface;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Notifies client that they have joined a party.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-27)
/// IDA Struct: PACKET_SC_PARTY_JOINED
/// Size: 54 bytes total (variable-length packet)
/// Memory Layout (IDA):
/// - 0x00: PACKET_VAR header (4 bytes) - handled by IPacketVar
/// - 0x04: char[41] name (41 bytes) - Party name
/// - 0x2D: unsigned int myIdx (4 bytes) - This player's member index
/// - 0x31: unsigned int leaderIndex (4 bytes) - Party leader's index
/// - 0x35: unsigned __int8 option (1 byte) - Party options/flags
/// Triggered by: CS_PARTY_JOIN, party invite accept
/// Note: Variable-length PARTY_MEMBER array may follow
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_PARTY_JOINED : IPacketVar
{
	/// <summary>Party name (offset 0x04, 41 bytes)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 41)]
	public byte[] name;

	/// <summary>This player's party member index (offset 0x2D)</summary>
	public uint myIdx;

	/// <summary>Party leader's member index (offset 0x31)</summary>
	public uint leaderIndex;

	/// <summary>Party options/flags (offset 0x35)</summary>
	public byte option;
}
