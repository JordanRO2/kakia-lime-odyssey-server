using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.AntiCheat;
using kakia_lime_odyssey_server.Constants;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Movement;

[PacketHandlerAttr(PacketType.CS_JUMP_PC)]
class CS_JUMP_PC_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		var cs_jump = PacketConverter.Extract<CS_JUMP_PC>(p.Payload);
		var playerClient = client as PlayerClient;

		if (playerClient == null)
			return;

		// Get player info for logging
		var playerName = playerClient.GetCurrentCharacter()?.appearance?.name ?? "Unknown";
		var playerId = client.GetObjInstID();

		// ANTI-CHEAT: Validate direction vector is normalized
		if (!MovementValidator.ValidateDirection(cs_jump.dir, out float dirLength))
		{
			CheatDetection.LogInvalidDirection(playerId, playerName, cs_jump.dir, dirLength);
			// Normalize the direction to prevent client desync
			cs_jump.dir = MovementValidator.Normalize(cs_jump.dir);
		}

		// ANTI-CHEAT: Detect fly hacking
		var lastPos = playerClient.GetLastPosition();
		if (lastPos.x != 0 || lastPos.y != 0 || lastPos.z != 0) // Skip first packet
		{
			if (MovementValidator.IsFlyHack(lastPos, cs_jump.pos, true, GameConstants.Movement.PC_MAX_JUMP_HEIGHT, out float heightDelta))
			{
				CheatDetection.LogFlyHack(playerId, playerName, lastPos.y, cs_jump.pos.y, heightDelta, GameConstants.Movement.PC_MAX_JUMP_HEIGHT);
				// Rubber-band player back to last valid position
				cs_jump.pos = lastPos;
			}
		}

		// Set jumping state (will be cleared on next CS_MOVE_PC or after timeout)
		playerClient.SetJumping(true);

		// Update tracking
		playerClient.UpdateLastPosition(cs_jump.pos);

		client.UpdatePosition(cs_jump.pos);
		client.UpdateDirection(cs_jump.dir);

		var vel = client.GetVelocities();

		SC_JUMP_PC sc_jump = new()
		{
			objInstID = client.GetObjInstID(),
			pos = cs_jump.pos,
			dir = cs_jump.dir,
			deltaLookAtRadian = cs_jump.deltaLookAtRadian,
			tick = LimeServer.GetCurrentTick(),
			velocity = client.IsInMotion() ? vel.run : 0,
			accel = client.IsInMotion() ? vel.runAccel : 0,
			ratio = client.IsInMotion() ? vel.ratio : 0,
			isSwim = cs_jump.isSwim
		};

		using PacketWriter pw = new();
		pw.Write(sc_jump);

		client.SendGlobalPacket(pw.ToPacket(), default);
	}
}
