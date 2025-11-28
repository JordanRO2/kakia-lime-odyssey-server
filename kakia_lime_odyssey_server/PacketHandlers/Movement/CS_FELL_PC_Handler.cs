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
/// </summary>
[PacketHandlerAttr(PacketType.CS_FELL_PC)]
class CS_FELL_PC_Handler : PacketHandler
{
	/// <summary>Minimum fall velocity to take damage.</summary>
	private const float FallDamageThreshold = 10.0f;

	/// <summary>Damage multiplier per unit of velocity over threshold.</summary>
	private const float FallDamageMultiplier = 5.0f;

	/// <summary>Maximum fall damage as percentage of max HP (90%).</summary>
	private const float MaxFallDamagePercent = 0.9f;

	/// <summary>
	/// Processes the fall packet and applies damage if necessary.
	/// </summary>
	/// <param name="client">The client that fell.</param>
	/// <param name="p">The raw packet data.</param>
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (client is not PlayerClient pc) return;

		var cs_fell = PacketConverter.Extract<CS_FELL_PC>(p.Payload);

		string playerName = pc.GetCurrentCharacter()?.appearance.name ?? "Unknown";
		Logger.Log($"[FELL] {playerName} landed with velocity: {cs_fell.velocity:F2}", LogLevel.Debug);

		// Check if velocity exceeds damage threshold
		if (cs_fell.velocity > FallDamageThreshold)
		{
			var status = client.GetStatus();

			// Calculate damage based on velocity over threshold
			float excessVelocity = cs_fell.velocity - FallDamageThreshold;
			uint fallDamage = (uint)(excessVelocity * FallDamageMultiplier);

			// Cap damage at percentage of max HP (prevents instant death from high falls)
			uint maxDamage = (uint)(status.mhp * MaxFallDamagePercent);
			if (fallDamage > maxDamage)
			{
				fallDamage = maxDamage;
			}

			// Calculate new HP (minimum 1 if capping is active, 0 if fatal)
			int newHP;
			if (fallDamage >= status.hp)
			{
				// Check if fall should be fatal
				if (excessVelocity > 50.0f) // Very high falls are fatal
				{
					newHP = 0;
				}
				else
				{
					newHP = 1; // Non-fatal falls leave player at 1 HP
				}
			}
			else
			{
				newHP = (int)(status.hp - fallDamage);
			}

			Logger.Log($"[FELL] {playerName} took {fallDamage} fall damage, HP: {status.hp} -> {newHP}", LogLevel.Information);

			// Apply HP change
			pc.UpdateHP(newHP, true);

			// Check for death
			if (newHP <= 0)
			{
				Logger.Log($"[FELL] {playerName} died from fall damage", LogLevel.Information);
				pc.SendDeath(); // Triggers death handling
			}
		}
	}
}
