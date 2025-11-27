using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Party;

[PacketHandlerAttr(PacketType.CS_PARTY_CHANGE_LEADER)]
class CS_PARTY_CHANGE_LEADER_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		var packet = PacketConverter.Extract<CS_PARTY_CHANGE_LEADER>(p.Payload);

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[PARTY] {playerName} promoting member index {packet.idx} to leader", LogLevel.Debug);

		bool success = LimeServer.PartyService.ChangeLeader(pc, packet.idx);

		if (!success)
		{
			Logger.Log($"[PARTY] {playerName} failed to change leader (not leader or invalid index)", LogLevel.Debug);
		}
	}
}
