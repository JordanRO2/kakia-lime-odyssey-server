namespace kakia_lime_odyssey_server.Models.FileHandler.Models;

// Model container used by MdiParser.Models
public class MdiModel
{
	// Populated for v1.8+ parsed models
	public string? Name { get; set; }
	public string? ResName { get; set; }
	public string? RelativeDir { get; set; }
	public string? TextureChangeInfoFile { get; set; }

	public Vec3? Pos { get; set; }
	public Vec3? Rot { get; set; }
	public Vec3? Scale { get; set; }
	public Bound? Bound { get; set; }
	public int? Water { get; set; }

	// Populated for legacy unparsed models (raw bytes)
	public byte[]? RawData { get; init; }
}

public record struct Vec3(float X, float Y, float Z);

public class Bound
{
	public Vec3 Center { get; init; }
	public float Radius { get; init; }
}