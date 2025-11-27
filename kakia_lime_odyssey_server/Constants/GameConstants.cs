namespace kakia_lime_odyssey_server.Constants;

/// <summary>
/// Game constants extracted from LimeOdyssey.exe client analysis
/// Base Address: 0x400000, MD5: 54052ef4b6b47a7e5516aab2d976ee8b
/// </summary>
public static class GameConstants
{
	/// <summary>
	/// Movement speed constants (from client strings at 0xa75650-0xa756e4)
	/// </summary>
	public static class Movement
	{
		/// <summary>Base run speed</summary>
		public const float PC_RUN_SPEED = 90.0f;

		/// <summary>Base walk speed</summary>
		public const float PC_WALK_SPEED = 80.0f;

		/// <summary>Backward walk speed</summary>
		public const float PC_WALK_BACK_SPEED = 70.0f;

		/// <summary>Jump speed</summary>
		public const float PC_JUMP_SPEED = 90.0f;

		/// <summary>Maximum jump height (vertical distance)</summary>
		public const float PC_MAX_JUMP_HEIGHT = 5.0f;

		/// <summary>Swimming speed</summary>
		public const float PC_SWIMMING_SPEED = 90.0f;

		/// <summary>Backward swimming speed</summary>
		public const float PC_SWIMMING_BACK_SPEED = 90.0f;

		/// <summary>Fall damage velocity threshold (from CS_FELL_PC_Handler)</summary>
		public const float FALL_DAMAGE_THRESHOLD = 10.0f;

		/// <summary>Fall damage multiplier</summary>
		public const float FALL_DAMAGE_MULTIPLIER = 5.0f;

		/// <summary>Tolerance for speed checks (10% for network latency)</summary>
		public const float SPEED_CHECK_TOLERANCE = 1.1f;

		/// <summary>Tolerance for direction normalization checks</summary>
		public const float DIRECTION_NORMAL_TOLERANCE = 0.01f;

		/// <summary>Maximum teleport distance before flagging as cheat (units)</summary>
		public const float MAX_TELEPORT_DISTANCE = 50.0f;
	}

	/// <summary>
	/// Combat and stat constants
	/// </summary>
	public static class Combat
	{
		/// <summary>Default attack range for melee</summary>
		public const float MELEE_ATTACK_RANGE = 3.0f;

		/// <summary>Maximum skill range</summary>
		public const float MAX_SKILL_RANGE = 30.0f;

		/// <summary>Critical hit multiplier</summary>
		public const float CRITICAL_HIT_MULTIPLIER = 1.5f;
	}

	/// <summary>
	/// Item and inventory constants
	/// </summary>
	public static class Inventory
	{
		/// <summary>Maximum stack size for stackable items</summary>
		public const int MAX_STACK_SIZE = 99;

		/// <summary>Default inventory size</summary>
		public const int DEFAULT_INVENTORY_SIZE = 48;
	}

	/// <summary>
	/// Network and timing constants
	/// </summary>
	public static class Network
	{
		/// <summary>Ping interval in milliseconds</summary>
		public const uint PING_INTERVAL_MS = 30000;

		/// <summary>Maximum packet rate (packets per second)</summary>
		public const int MAX_PACKET_RATE = 100;

		/// <summary>Client tick rate (milliseconds per tick)</summary>
		public const float MS_PER_TICK = 1.0f;
	}

	/// <summary>
	/// Map boundary constants
	/// Note: These are default boundaries. Actual boundaries should be loaded from map data.
	/// </summary>
	public static class MapBounds
	{
		/// <summary>Default minimum X coordinate</summary>
		public const float DEFAULT_MIN_X = -2048.0f;

		/// <summary>Default maximum X coordinate</summary>
		public const float DEFAULT_MAX_X = 2048.0f;

		/// <summary>Default minimum Y coordinate (height)</summary>
		public const float DEFAULT_MIN_Y = -100.0f;

		/// <summary>Default maximum Y coordinate (height)</summary>
		public const float DEFAULT_MAX_Y = 1000.0f;

		/// <summary>Default minimum Z coordinate</summary>
		public const float DEFAULT_MIN_Z = -2048.0f;

		/// <summary>Default maximum Z coordinate</summary>
		public const float DEFAULT_MAX_Z = 2048.0f;
	}
}
