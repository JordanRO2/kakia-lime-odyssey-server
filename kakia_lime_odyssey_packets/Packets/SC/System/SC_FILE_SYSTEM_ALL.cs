/// <summary>
/// SC_ versions of file system packets without PACKET_FIX/PACKET_VAR header.
/// These are used for server-side packet sending.
/// </summary>
using System.Runtime.InteropServices;
using kakia_lime_odyssey_packets.Packets.Interface;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_FILE_LIST (variable)
/// Contains file information for download.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_FILE_LIST : IPacketVar
{
	// Variable length file list data follows
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_DOWNLOAD_FILE_BLOCK (2 bytes header only)
/// Contains a block of file data being downloaded.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_DOWNLOAD_FILE_BLOCK : IPacketFixed
{
	// File block data follows in payload
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_UPLOAD_NEXT_FILE_BLOCK (2 bytes header only)
/// Acknowledges receipt of upload block and requests next.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_UPLOAD_NEXT_FILE_BLOCK : IPacketFixed
{
	// No payload - acknowledgment only
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_START_DOWNLOAD_FILE (2 bytes header only)
/// Initiates a file download to the client.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_START_DOWNLOAD_FILE : IPacketFixed
{
	// No payload - acknowledgment only
}

/// <summary>
/// IDA Verified: 2025-11-26
/// Structure: PACKET_SC_START_UPLOAD_FILE (2 bytes header only)
/// Acknowledges file upload initiation from client.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_START_UPLOAD_FILE : IPacketFixed
{
	// No payload - acknowledgment only
}

// SC_CHANGE_SCALE is defined in SC_CHANGE_SCALE.cs
