using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server->Client packet updating guild name visibility for a character in sight.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_GUILD_NAME_UPDATE_SIGHT
/// Size: 61 bytes total (2 byte header + 51 byte name + 8 byte instID)
/// Triggered by: Character enters sight with guild tag
/// Sent to: Players who can see the character
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct SC_GUILD_NAME_UPDATE_SIGHT
{
	/// <summary>Guild name (max 50 chars + null terminator)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 51)]
	public byte[] name;

	/// <summary>Padding for alignment</summary>
	private byte padding;

	/// <summary>Character instance ID</summary>
	public long instID;
}
