/// <summary>
/// Handler for CS_DOWNLOAD_FILE packet - client requests file download.
/// </summary>
/// <remarks>
/// This is a legacy file transfer packet not used in normal gameplay.
/// The server does not support file downloads, so this handler logs and ignores.
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;

namespace kakia_lime_odyssey_server.PacketHandlers.System;

/// <summary>
/// Handles file download requests from the client.
/// </summary>
[PacketHandlerAttr(PacketType.CS_DOWNLOAD_FILE)]
class CS_DOWNLOAD_FILE_Handler : PacketHandler
{
	/// <summary>
	/// Processes the file download request.
	/// </summary>
	/// <param name="client">The client connection</param>
	/// <param name="p">The raw packet data</param>
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		// File transfer is not supported - log and ignore
		Logger.Log($"[FileTransfer] Client requested file download - not supported", LogLevel.Debug);
	}
}
