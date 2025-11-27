namespace kakia_lime_odyssey_server.Services.Combat;

/// <summary>
/// Service for calculating elemental damage bonuses.
/// Note: The Korean CBT3 client does not appear to have a fully implemented elemental system,
/// but the HIT_DESC structure has a bonusDamage field that could be used for elemental damage.
/// This service provides a framework for elemental damage if it's needed in the future.
/// </summary>
public static class ElementalDamageService
{
	/// <summary>
	/// Elemental damage types.
	/// </summary>
	public enum ElementType
	{
		None = 0,
		Fire = 1,
		Ice = 2,
		Lightning = 3,
		Earth = 4,
		Wind = 5,
		Water = 6,
		Light = 7,
		Dark = 8
	}

	/// <summary>
	/// Element weakness/resistance multipliers.
	/// Row = attacker element, Column = defender element.
	/// Values > 1.0 mean weakness (bonus damage), < 1.0 mean resistance (reduced damage).
	/// </summary>
	private static readonly float[,] ElementalChart = new float[9, 9]
	{
		// None  Fire  Ice   Light Earth Wind  Water Light Dark
		{ 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f }, // None (neutral to all)
		{ 1.0f, 0.5f, 1.5f, 0.75f, 1.25f, 1.0f, 0.5f, 1.0f, 1.0f }, // Fire (weak to ice, resists water)
		{ 1.0f, 0.5f, 0.5f, 1.25f, 1.0f, 1.25f, 1.5f, 1.0f, 1.0f }, // Ice (weak to fire, strong vs wind)
		{ 1.0f, 1.25f, 0.75f, 0.5f, 1.5f, 1.0f, 1.25f, 1.0f, 1.0f }, // Lightning (weak to earth)
		{ 1.0f, 0.75f, 1.0f, 0.5f, 0.5f, 1.5f, 1.0f, 1.0f, 1.0f }, // Earth (weak to wind)
		{ 1.0f, 1.0f, 0.75f, 1.0f, 0.5f, 0.5f, 1.25f, 1.0f, 1.0f }, // Wind (weak to ice)
		{ 1.0f, 1.5f, 0.5f, 0.75f, 1.0f, 0.75f, 0.5f, 1.0f, 1.0f }, // Water (strong vs fire)
		{ 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 1.5f }, // Light (strong vs dark)
		{ 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.5f, 0.5f }  // Dark (strong vs light)
	};

	/// <summary>
	/// Calculates the elemental damage multiplier based on attacker and defender elements.
	/// </summary>
	/// <param name="attackerElement">The element of the attack</param>
	/// <param name="defenderElement">The element of the defender</param>
	/// <returns>Damage multiplier (1.0 = normal, >1.0 = bonus, <1.0 = resisted)</returns>
	public static float GetElementalMultiplier(ElementType attackerElement, ElementType defenderElement)
	{
		int attackIndex = (int)attackerElement;
		int defendIndex = (int)defenderElement;

		// Bounds check
		if (attackIndex < 0 || attackIndex > 8 || defendIndex < 0 || defendIndex > 8)
			return 1.0f;

		return ElementalChart[attackIndex, defendIndex];
	}

	/// <summary>
	/// Calculates bonus elemental damage.
	/// </summary>
	/// <param name="baseDamage">The base physical or magical damage</param>
	/// <param name="attackerElement">The element of the attack</param>
	/// <param name="defenderElement">The element of the defender</param>
	/// <param name="elementalPower">Base elemental power (e.g., from weapon or skill)</param>
	/// <returns>Bonus elemental damage to add</returns>
	public static uint CalculateElementalBonus(uint baseDamage, ElementType attackerElement, ElementType defenderElement, int elementalPower)
	{
		if (attackerElement == ElementType.None || elementalPower <= 0)
			return 0;

		float multiplier = GetElementalMultiplier(attackerElement, defenderElement);

		// Elemental bonus is a percentage of base damage modified by element chart
		// elementalPower acts as a percentage (e.g., 10 = 10% of base damage as elemental)
		float bonusDamage = baseDamage * (elementalPower / 100.0f) * multiplier;

		return (uint)Math.Max(0, bonusDamage);
	}

	/// <summary>
	/// Gets the name of an element type.
	/// </summary>
	public static string GetElementName(ElementType element)
	{
		return element switch
		{
			ElementType.Fire => "Fire",
			ElementType.Ice => "Ice",
			ElementType.Lightning => "Lightning",
			ElementType.Earth => "Earth",
			ElementType.Wind => "Wind",
			ElementType.Water => "Water",
			ElementType.Light => "Light",
			ElementType.Dark => "Dark",
			_ => "None"
		};
	}

	/// <summary>
	/// Checks if an element is strong against another element.
	/// </summary>
	public static bool IsStrongAgainst(ElementType attacker, ElementType defender)
	{
		return GetElementalMultiplier(attacker, defender) > 1.0f;
	}

	/// <summary>
	/// Checks if an element is weak against another element.
	/// </summary>
	public static bool IsWeakAgainst(ElementType attacker, ElementType defender)
	{
		return GetElementalMultiplier(attacker, defender) < 1.0f;
	}

	/// <summary>
	/// Gets the opposing element for a given element.
	/// </summary>
	public static ElementType GetOpposingElement(ElementType element)
	{
		return element switch
		{
			ElementType.Fire => ElementType.Water,
			ElementType.Ice => ElementType.Fire,
			ElementType.Lightning => ElementType.Earth,
			ElementType.Earth => ElementType.Wind,
			ElementType.Wind => ElementType.Ice,
			ElementType.Water => ElementType.Lightning,
			ElementType.Light => ElementType.Dark,
			ElementType.Dark => ElementType.Light,
			_ => ElementType.None
		};
	}
}
