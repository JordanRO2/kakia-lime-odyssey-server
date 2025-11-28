/// <summary>
/// Server packet containing a list of files available for download.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_FILE_LIST
/// Size: 4 bytes (base header)
/// Ordinal: 2415
/// Variable-length packet that contains file information.
/// The actual file list data follows the PACKET_VAR header.
/// Sent by server to inform client of available files for download.
/// </remarks>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Common;
using kakia_lime_odyssey_packets.Packets.Models;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_SC_FILE_LIST
{
	/// <summary>Variable-length packet header (contains opcode and total size)</summary>
	public PACKET_VAR header;
}
