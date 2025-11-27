using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Social;

[PacketHandlerAttr(PacketType.CS_WHISPER_PC)]
class CS_WHISPER_PC_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		using PacketReader pr = new PacketReader(p.Payload);
		var whisper = pr.Read_CS_WHISPER_PC(p.Size);

		string senderName = client.GetCurrentCharacter()?.appearance.name ?? "Unknown";

		Logger.Log($"[WHISPER] {senderName} -> {whisper.targetPCName}: {whisper.message}", LogLevel.Debug);

		// Validate message
		if (string.IsNullOrWhiteSpace(whisper.message))
		{
			Logger.Log($"[WHISPER] Empty message from {senderName}", LogLevel.Debug);
			return;
		}

		// Validate target name
		if (string.IsNullOrWhiteSpace(whisper.targetPCName))
		{
			Logger.Log($"[WHISPER] Empty target name from {senderName}", LogLevel.Debug);
			return;
		}

		// Cannot whisper to yourself
		if (whisper.targetPCName.Equals(senderName, StringComparison.OrdinalIgnoreCase))
		{
			Logger.Log($"[WHISPER] {senderName} tried to whisper to themselves", LogLevel.Debug);
			return;
		}

		// Find the target player
		var targetPlayer = FindPlayerByName(whisper.targetPCName);

		if (targetPlayer == null)
		{
			// Target player is offline or doesn't exist
			SendWhisperFailure(client, whisper.targetPCName);
			return;
		}

		// Send the whisper to the target
		SendWhisperToTarget(targetPlayer, senderName, whisper.message);

		// Send confirmation back to sender (echo the whisper)
		SendWhisperConfirmation(client, whisper.targetPCName, whisper.message);
	}

	/// <summary>
	/// Find an online player by their character name.
	/// </summary>
	private static PlayerClient? FindPlayerByName(string characterName)
	{
		return LimeServer.PlayerClients.FirstOrDefault(pc =>
		{
			var character = pc.GetCurrentCharacter();
			if (character == null)
				return false;
			return character.appearance.name.Equals(characterName, StringComparison.OrdinalIgnoreCase);
		});
	}

	/// <summary>
	/// Send the whisper message to the target player.
	/// </summary>
	private static void SendWhisperToTarget(PlayerClient target, string senderName, string message)
	{
		SC_WHISPER whisperPacket = new()
		{
			fromName = senderName,
			message = message
		};

		using PacketWriter pw = new();
		pw.Write(whisperPacket);
		target.Send(pw.ToSizedPacket(), default).Wait();

		Logger.Log($"[WHISPER] Delivered message from {senderName} to {target.GetCurrentCharacter()?.appearance.name}", LogLevel.Debug);
	}

	/// <summary>
	/// Send confirmation to the sender that their whisper was delivered.
	/// Uses the same SC_WHISPER packet but with target name as "from" to indicate outgoing.
	/// </summary>
	private static void SendWhisperConfirmation(IPlayerClient sender, string targetName, string message)
	{
		// Some clients expect a confirmation packet echoing the sent message
		// This allows the sender to see their own message in the chat log
		SC_WHISPER confirmPacket = new()
		{
			fromName = $"To {targetName}",
			message = message
		};

		using PacketWriter pw = new();
		pw.Write(confirmPacket);
		sender.Send(pw.ToSizedPacket(), default).Wait();
	}

	/// <summary>
	/// Send a failure notification when the target player is not found.
	/// </summary>
	private static void SendWhisperFailure(IPlayerClient sender, string targetName)
	{
		// Send a system message indicating the player is offline
		SC_WHISPER failPacket = new()
		{
			fromName = "System",
			message = $"Player '{targetName}' is not online."
		};

		using PacketWriter pw = new();
		pw.Write(failPacket);
		sender.Send(pw.ToSizedPacket(), default).Wait();

		Logger.Log($"[WHISPER] Target player '{targetName}' not found", LogLevel.Debug);
	}
}
