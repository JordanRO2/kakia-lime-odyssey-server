/// <summary>
/// Handles CS_FELL_PC packet - player landed after falling.
/// </summary>
/// <remarks>
/// Triggered by: Client landing after a fall
/// Response packets: SC_UPDATE_STATUS (HP update), SC_DEAD (if killed by fall)
/// Calculates fall damage based on falling velocity.
/// </remarks>
using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Movement;

/// <summary>
/// Handles fall damage when a player lands after falling.
/// Note: Fall damage is disabled for tuning. Normal jumps report velocity ~80.
/// </summary>
[PacketHandlerAttr(PacketType.CS_FELL_PC)]
class CS_FELL_PC_Handler : PacketHandler
{
	/// <summary>
	/// Processes the fall packet and logs velocity for tuning.
	/// Fall damage is currently disabled until proper thresholds are determined.
	/// </summary>
	/// <param name="client">The client that fell.</param>
	/// <param name="p">The raw packet data.</param>
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		var cs_fell = PacketConverter.Extract<CS_FELL_PC>(p.Payload);

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[FELL] {playerName} landed with velocity: {cs_fell.velocity:F2}", LogLevel.Debug);

		// Fall damage disabled for tuning - only logging velocity values
		// TODO: Enable fall damage once proper thresholds are determined
		// Normal jumps report velocity ~80, so threshold needs to be >100
	}
}
