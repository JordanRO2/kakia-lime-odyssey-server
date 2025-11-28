using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_server.Constants;

namespace kakia_lime_odyssey_server.AntiCheat;

/// <summary>
/// Movement validation logic extracted from client analysis
/// Implements anti-cheat checks for speed hacking, teleporting, and invalid movement
/// </summary>
public static class MovementValidator
{
	/// <summary>
	/// Validate movement speed between two positions
	/// </summary>
	/// <returns>True if movement is valid, false if speed hack detected</returns>
	public static bool ValidateSpeed(FPOS oldPos, FPOS newPos, uint oldTick, uint newTick, float maxSpeed, out float actualSpeed)
	{
		// Calculate time delta in seconds
		float deltaTime = (newTick - oldTick) / 1000.0f;

		// Avoid division by zero or negative time
		if (deltaTime <= 0.001f)
		{
			actualSpeed = 0;
			return true; // Too small time delta to validate
		}

		// Calculate distance
		float distance = CalculateDistance(oldPos, newPos);
		actualSpeed = distance / deltaTime;

		// Check against max speed with tolerance for network latency
		float allowedSpeed = maxSpeed * GameConstants.Movement.SPEED_CHECK_TOLERANCE;
		return actualSpeed <= allowedSpeed;
	}

	/// <summary>
	/// Validate direction vector is normalized (client requirement)
	/// Client checks: "AlmostSame(moveDir.Length(), 1.0f)" at 0xa2bb8c
	/// </summary>
	public static bool ValidateDirection(FPOS direction, out float length)
	{
		length = CalculateLength(direction);

		// Direction must be normalized (length = 1.0)
		float diff = Math.Abs(length - 1.0f);
		return diff < GameConstants.Movement.DIRECTION_NORMAL_TOLERANCE;
	}

	/// <summary>
	/// Detect teleport hacking (instant position change beyond reasonable distance)
	/// </summary>
	public static bool IsTeleport(FPOS oldPos, FPOS newPos, out float distance)
	{
		distance = CalculateDistance(oldPos, newPos);
		return distance > GameConstants.Movement.MAX_TELEPORT_DISTANCE;
	}

	/// <summary>
	/// Validate tick progression (prevent packet replay attacks)
	/// </summary>
	public static bool ValidateTickProgression(uint oldTick, uint newTick, out int tickDiff)
	{
		tickDiff = (int)(newTick - oldTick);

		// Tick must progress forward
		if (tickDiff <= 0)
			return false;

		// Reasonable tick delta (max 10 seconds = 10000 ticks)
		if (tickDiff > 10000)
			return false;

		return true;
	}

	/// <summary>
	/// Calculate 3D distance between two positions
	/// </summary>
	private static float CalculateDistance(FPOS pos1, FPOS pos2)
	{
		float dx = pos2.x - pos1.x;
		float dy = pos2.y - pos1.y;
		float dz = pos2.z - pos1.z;
		return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
	}

	/// <summary>
	/// Calculate vector length
	/// </summary>
	private static float CalculateLength(FPOS vec)
	{
		return (float)Math.Sqrt(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z);
	}

	/// <summary>
	/// Normalize a direction vector
	/// </summary>
	public static FPOS Normalize(FPOS vec)
	{
		float length = CalculateLength(vec);
		if (length < 0.0001f)
			return new FPOS { x = 0, y = 0, z = 0 };

		return new FPOS
		{
			x = vec.x / length,
			y = vec.y / length,
			z = vec.z / length
		};
	}

	/// <summary>
	/// Detect fly hacking by checking vertical movement
	/// Note: In Lime Odyssey, Z is the vertical/height coordinate (not Y)
	/// </summary>
	/// <param name="oldPos">Previous position</param>
	/// <param name="newPos">New position</param>
	/// <param name="isJumping">Whether player is currently jumping</param>
	/// <param name="maxJumpHeight">Maximum allowed jump height</param>
	/// <param name="heightDelta">Output: actual height change</param>
	/// <returns>True if fly hack detected</returns>
	public static bool IsFlyHack(FPOS oldPos, FPOS newPos, bool isJumping, float maxJumpHeight, out float heightDelta)
	{
		// Z is the height coordinate in Lime Odyssey (not Y)
		heightDelta = newPos.z - oldPos.z;

		// Ignore downward movement (falling is always allowed)
		if (heightDelta <= 0)
			return false;

		// If jumping is allowed, check against max jump height
		if (isJumping)
		{
			return heightDelta > maxJumpHeight;
		}

		// If not jumping, any significant upward movement is suspicious
		// Allow small height changes for terrain/stairs (e.g., 2 units)
		const float TERRAIN_HEIGHT_TOLERANCE = 2.0f;
		return heightDelta > TERRAIN_HEIGHT_TOLERANCE;
	}

	/// <summary>
	/// Check if position is within valid map boundaries
	/// </summary>
	/// <param name="pos">Position to check</param>
	/// <param name="minBounds">Minimum boundary coordinates</param>
	/// <param name="maxBounds">Maximum boundary coordinates</param>
	/// <returns>True if position is out of bounds</returns>
	public static bool IsOutOfBounds(FPOS pos, FPOS minBounds, FPOS maxBounds)
	{
		if (pos.x < minBounds.x || pos.x > maxBounds.x)
			return true;

		if (pos.y < minBounds.y || pos.y > maxBounds.y)
			return true;

		if (pos.z < minBounds.z || pos.z > maxBounds.z)
			return true;

		return false;
	}

	/// <summary>
	/// Calculate 2D horizontal distance (ignore Z/height)
	/// Note: In Lime Odyssey, X and Y are horizontal, Z is height
	/// Used for ground-based movement validation
	/// </summary>
	public static float Calculate2DDistance(FPOS pos1, FPOS pos2)
	{
		float dx = pos2.x - pos1.x;
		float dy = pos2.y - pos1.y;
		return (float)Math.Sqrt(dx * dx + dy * dy);
	}
}
