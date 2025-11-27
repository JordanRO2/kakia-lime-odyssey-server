/// <summary>
/// Handles CS_CHANGE_HELM_SHOWMODE packet - player toggles helmet visibility.
/// </summary>
/// <remarks>
/// Triggered by: Player toggling helmet visibility in UI
/// Response packets: SC_CHANGE_HELM_SHOWMODE
/// Database: characters (update helm show mode)
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Character;

[PacketHandlerAttr(PacketType.CS_CHANGE_HELM_SHOWMODE)]
class CS_CHANGE_HELM_SHOWMODE_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		using PacketReader pr = new(p.Payload);
		var packet = pr.Read<CS_CHANGE_HELM_SHOWMODE>();

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[CHAR] {playerName} changing helmet visibility to {packet.show}", LogLevel.Debug);

		// Update character's helm show mode (saved on next character save)
		var character = pc.GetCurrentCharacter();
		if (character != null)
		{
			character.appearance.helmShowMode = packet.show;
		}

		// Send confirmation and broadcast to others
		SC_CHANGE_HELM_SHOWMODE response = new()
		{
			instID = pc.GetObjInstID(),
			show = packet.show
		};

		using PacketWriter pw = new();
		pw.Write(response);
		pc.Send(pw.ToPacket(), default).Wait();
		// Broadcast to others so they see the visibility change
		pc.SendGlobalPacket(pw.ToPacket(), default).Wait();

		Logger.Log($"[CHAR] {playerName} helmet visibility set to {packet.show}", LogLevel.Debug);
	}
}
