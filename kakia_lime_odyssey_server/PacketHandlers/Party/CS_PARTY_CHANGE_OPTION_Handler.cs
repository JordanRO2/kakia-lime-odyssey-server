using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_server.Network;
using kakia_lime_odyssey_server.Services.Party;

namespace kakia_lime_odyssey_server.PacketHandlers.Party;

[PacketHandlerAttr(PacketType.CS_PARTY_CHANGE_OPTION)]
class CS_PARTY_CHANGE_OPTION_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		using PacketReader pr = new(p.Payload);
		var packet = pr.Read<CS_PARTY_CHANGE_OPTION>();

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[PARTY] {playerName} changing party option to {packet.type}", LogLevel.Debug);

		bool success = LimeServer.PartyService.ChangeOption(pc, (PartyOption)packet.type);

		if (!success)
		{
			Logger.Log($"[PARTY] {playerName} failed to change option (not leader)", LogLevel.Debug);
		}
	}
}
