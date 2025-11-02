using System.Numerics;

namespace kakia_lime_odyssey_server.Models.FileHandler.Terrain;

/// <summary>
/// Defines the terrain quadrant this file represents.
/// </summary>
public enum QuadrantType
{
	QT_LEFT_TOP = 0,
	QT_LEFT_BOTTOM = 1,
	QT_RIGHT_TOP = 2,
	QT_RIGHT_BOTTOM = 3,
	QT_ALL = 4
}

public class ParsedTrnData
{
	public QuadrantType Quadrant { get; set; }
	public string? Magic { get; set; }
	public (int Major, int Minor) Version { get; set; }
	public float CellSize { get; set; }
	public int TerrainShiftFile { get; set; }

	// Arrays
	public float[,]? HeightTable { get; set; }
	public float[,,]? NormalTable { get; set; } // [y, x, (r,g,b)]
	public uint[,]? MixPixels1 { get; set; }
	public uint[,]? MixPixels2 { get; set; }
	public byte[,,]? LightmapData { get; set; } // [y, x, (r,g,b,a)]

	public int LightmapTexelPerCell { get; set; }

	// Other data
	public List<(int IsPunched, byte[]? PunchData)>? PunchInfo { get; set; }
	public (Vector3 Center, float Radius) MainBound { get; set; }
	public byte[]? SectorBoundsData { get; set; }
	public byte[]? SectorLayerData { get; set; }
	public List<string>? LayerTextureNames { get; set; }
}