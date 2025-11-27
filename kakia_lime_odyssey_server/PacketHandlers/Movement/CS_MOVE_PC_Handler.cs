using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_packets.Packets.Enums;
using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.AntiCheat;
using kakia_lime_odyssey_server.Constants;
using kakia_lime_odyssey_server.Network;

namespace kakia_lime_odyssey_server.PacketHandlers.Movement;

[PacketHandlerAttr(PacketType.CS_MOVE_PC)]
class CS_MOVE_PC_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		var move_pc = PacketConverter.Extract<CS_MOVE_PC>(p.Payload);
		var vel = client.GetVelocities();
		var playerClient = client as PlayerClient;

		if (playerClient == null)
			return;

		// Get player info for logging
		var playerName = playerClient.GetCurrentCharacter()?.appearance?.name ?? "Unknown";
		var playerId = client.GetObjInstID();

		// ANTI-CHEAT: Validate direction vector is normalized
		if (!MovementValidator.ValidateDirection(move_pc.dir, out float dirLength))
		{
			CheatDetection.LogInvalidDirection(playerId, playerName, move_pc.dir, dirLength);
			// Normalize the direction to prevent client desync
			move_pc.dir = MovementValidator.Normalize(move_pc.dir);
		}

		// ANTI-CHEAT: Validate tick progression
		uint lastTick = playerClient.GetLastClientTick();
		if (lastTick > 0) // Skip first packet
		{
			if (!MovementValidator.ValidateTickProgression(lastTick, move_pc.tick, out int tickDiff))
			{
				CheatDetection.LogTickManipulation(playerId, playerName, move_pc.tick, lastTick);
				// Reject packet with suspicious tick
				return;
			}
		}

		// ANTI-CHEAT: Validate movement speed
		FPOS lastPos = playerClient.GetLastPosition();
		if (lastTick > 0 && (lastPos.x != 0 || lastPos.y != 0 || lastPos.z != 0)) // Skip first packet
		{
			float maxSpeed = playerClient.GetMaxSpeed(move_pc.moveType);
			if (!MovementValidator.ValidateSpeed(lastPos, move_pc.pos, lastTick, move_pc.tick, maxSpeed, out float actualSpeed))
			{
				float distance = (float)Math.Sqrt(
					Math.Pow(move_pc.pos.x - lastPos.x, 2) +
					Math.Pow(move_pc.pos.y - lastPos.y, 2) +
					Math.Pow(move_pc.pos.z - lastPos.z, 2)
				);
				float deltaTime = (move_pc.tick - lastTick) / 1000.0f;

				CheatDetection.LogSpeedHack(playerId, playerName, actualSpeed, maxSpeed, distance, deltaTime);

				// Rubber-band player back to last valid position
				move_pc.pos = lastPos;
			}

			// ANTI-CHEAT: Detect teleport hacking
			if (MovementValidator.IsTeleport(lastPos, move_pc.pos, out float teleportDistance))
			{
				CheatDetection.LogTeleport(playerId, playerName, lastPos, move_pc.pos, teleportDistance);
				// Rubber-band player back
				move_pc.pos = lastPos;
			}

			// ANTI-CHEAT: Detect fly hacking (upward movement without jumping)
			// Check if player is jumping and if jump duration is still valid (max 2 seconds)
			bool isJumpingValid = playerClient.IsJumping() && playerClient.GetJumpDuration().TotalSeconds < 2.0;

			if (MovementValidator.IsFlyHack(lastPos, move_pc.pos, isJumpingValid, GameConstants.Movement.PC_MAX_JUMP_HEIGHT, out float heightDelta))
			{
				CheatDetection.LogFlyHack(playerId, playerName, lastPos.y, move_pc.pos.y, heightDelta, GameConstants.Movement.PC_MAX_JUMP_HEIGHT);
				// Rubber-band player back to last valid position
				move_pc.pos = lastPos;
			}

			// Clear jumping state if player is moving downward (landed)
			if (heightDelta < 0 && playerClient.IsJumping())
			{
				playerClient.SetJumping(false);
			}

			// ANTI-CHEAT: Detect out of bounds
			FPOS minBounds = new FPOS
			{
				x = GameConstants.MapBounds.DEFAULT_MIN_X,
				y = GameConstants.MapBounds.DEFAULT_MIN_Y,
				z = GameConstants.MapBounds.DEFAULT_MIN_Z
			};
			FPOS maxBounds = new FPOS
			{
				x = GameConstants.MapBounds.DEFAULT_MAX_X,
				y = GameConstants.MapBounds.DEFAULT_MAX_Y,
				z = GameConstants.MapBounds.DEFAULT_MAX_Z
			};

			if (MovementValidator.IsOutOfBounds(move_pc.pos, minBounds, maxBounds))
			{
				CheatDetection.LogOutOfBounds(playerId, playerName, move_pc.pos, "map boundaries");
				// Rubber-band player back to last valid position
				move_pc.pos = lastPos;
			}
		}

		// Update anti-cheat tracking
		playerClient.UpdateClientTick(move_pc.tick);
		playerClient.UpdateLastPosition(move_pc.pos);
		playerClient.UpdateLastMoveTime();
		playerClient.IncrementMovePacketCount();

		// Broadcast movement to other players
		SC_MOVE sc_move = new()
		{
			objInstID = client.GetObjInstID(),
			pos = move_pc.pos,
			dir = move_pc.dir,
			deltaLookAtRadian = move_pc.deltaLookAtRadian,
			tick = LimeServer.GetCurrentTick(),
			moveType = move_pc.moveType,
			turningSpeed = 1,
			accel = GetAcceleration((MOVE_TYPE)move_pc.moveType, vel),
			velocity = GetVelocity((MOVE_TYPE)move_pc.moveType, vel),
			velocityRatio = vel.ratio
		};

		client.UpdatePosition(move_pc.pos);
		client.UpdateDirection(move_pc.dir);
		client.SetInMotion(true);

		using PacketWriter pw = new();
		pw.Write(sc_move);
		client.SendGlobalPacket(pw.ToPacket(), default).Wait();
	}

	public float GetAcceleration(MOVE_TYPE type, VELOCITIES vel)
	{
		return type switch
		{
			MOVE_TYPE.MOVE_TYPE_WALK => vel.walkAccel,
			MOVE_TYPE.MOVE_TYPE_RUN => vel.runAccel,
			MOVE_TYPE.MOVE_TYPE_SWIM => vel.swimAccel,
			_ => 1
		};
	}

	public float GetVelocity(MOVE_TYPE type, VELOCITIES vel)
	{
		return type switch
		{
			MOVE_TYPE.MOVE_TYPE_WALK => vel.walk,
			MOVE_TYPE.MOVE_TYPE_RUN => vel.run,
			MOVE_TYPE.MOVE_TYPE_SWIM => vel.swim,
			_ => 1
		};
	}
}
