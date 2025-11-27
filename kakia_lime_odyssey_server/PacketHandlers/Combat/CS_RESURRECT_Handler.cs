/// <summary>
/// Handles CS_RESURRECT packet - player resurrects at current location.
/// </summary>
/// <remarks>
/// Triggered by: Player choosing to resurrect in place (resurrection stone/skill)
/// Response packets: SC_RESURRECTED
/// Note: Different from CS_ESCAPE which returns player to town
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Combat;

[PacketHandlerAttr(PacketType.CS_RESURRECT)]
class CS_RESURRECT_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		var character = pc.GetCurrentCharacter();
		if (character == null) return;

		string playerName = character.appearance.name ?? "Unknown";

		// Check if player is actually dead
		var status = pc.GetStatus();
		if (status.hp > 0)
		{
			Logger.Log($"[RESURRECT] {playerName} attempted to resurrect but is not dead (HP: {status.hp})", LogLevel.Warning);
			return;
		}

		Logger.Log($"[RESURRECT] {playerName} resurrecting at current location", LogLevel.Debug);

		// Calculate resurrection HP (typically 10-30% of max HP)
		uint resurrectHP = Math.Max(1, status.mhp / 5); // 20% of max HP

		// Update player HP
		pc.UpdateHP((int)resurrectHP, false);

		// Send resurrection packet to player and nearby players
		var packet = new SC_RESURRECTED
		{
			objInstID = pc.GetObjInstID(),
			hp = resurrectHP
		};

		using PacketWriter pw = new();
		pw.Write(packet);
		var packetBytes = pw.ToSizedPacket();

		// Send to player
		pc.Send(packetBytes, default).Wait();

		// Broadcast to nearby players
		pc.SendGlobalPacket(packetBytes, default).Wait();

		Logger.Log($"[RESURRECT] {playerName} resurrected with {resurrectHP} HP", LogLevel.Information);
	}
}
