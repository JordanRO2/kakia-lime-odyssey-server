using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_server.Models;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Movement;

[PacketHandlerAttr(PacketType.CS_FELL_PC)]
class CS_FELL_PC_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		var cs_fell = PacketConverter.Extract<CS_FELL_PC>(p.Payload);

		Logger.Log($"[CS_FELL_PC] Player {client.GetObjInstID()} fell with velocity: {cs_fell.velocity}");

		const float FALL_DAMAGE_THRESHOLD = 10.0f;
		const float FALL_DAMAGE_MULTIPLIER = 5.0f;

		if (cs_fell.velocity > FALL_DAMAGE_THRESHOLD)
		{
			uint fallDamage = (uint)((cs_fell.velocity - FALL_DAMAGE_THRESHOLD) * FALL_DAMAGE_MULTIPLIER);
			var status = client.GetStatus();
			int newHP = (int)(status.hp > fallDamage ? status.hp - fallDamage : 0);

			Logger.Log($"[CS_FELL_PC] Fall damage: {fallDamage}, HP: {status.hp} -> {newHP}");

			(client as PlayerClient)!.UpdateHP(newHP, true);
		}
	}
}
