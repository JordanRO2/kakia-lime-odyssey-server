using System.Drawing;
using System.Drawing.Imaging;

namespace kakia_lime_odyssey_server.Models.FileHandler.Terrain;

public class FullTerrain
{
	public List<ParsedTrnData> Quadrants { get; set; }

	public FullTerrain(string mapName, string folder)
	{
		var files = Directory.GetFiles(folder, $"{mapName}.trn*");
		Quadrants = new List<ParsedTrnData>();
		foreach (var file in files)
		{
			var trnParser = new TrnParser(file, QuadrantType.QT_ALL);
			trnParser.Parse();

			trnParser.Data.Quadrant = (QuadrantType)int.Parse(file.Last().ToString());
			Quadrants.Add(trnParser.Data);
		}
	}

	/// <summary>
	/// Save a composite bitmap that encodes normals, modulated by lightmap and height.
	/// Quadrants are placed according to ParsedTrnData.Quadrant:
	/// QT_LEFT_TOP, QT_LEFT_BOTTOM, QT_RIGHT_TOP, QT_RIGHT_BOTTOM.
	/// Missing quadrants are rendered as black.
	/// </summary>
	/// <param name="outputPath">File path to save (png suggested).</param>
	/// <param name="downsample">Sampling step. 1 = full resolution.</param>
	/// <param name="maxWidth">Maximum output width in pixels (after downsample).</param>
	public bool SaveNormalLightHeightBitmap(string outputPath, int downsample = 1, int maxWidth = 4096)
	{
		if (Quadrants == null || Quadrants.Count == 0)
		{
			Console.WriteLine("No quadrants loaded to generate bitmap.");
			return false;
		}

		// Find a quadrant that has a normal table to derive per-quadrant size
		ParsedTrnData? sample = null;
		foreach (var q in Quadrants)
		{
			if (q?.NormalTable != null) { sample = q; break; }
		}
		if (sample == null)
		{
			Console.WriteLine("No normal map data available.");
			return false;
		}

		int qHeight = sample.NormalTable.GetLength(0);
		int qWidth = sample.NormalTable.GetLength(1);

		// Build a 2x2 grid based on the Quadrant enum positions
		// grid[row, col] where row 0 = top, 1 = bottom; col 0 = left, 1 = right
		var grid = new ParsedTrnData?[2, 2];
		foreach (var q in Quadrants)
		{
			if (q == null) continue;
			switch (q.Quadrant)
			{
				case QuadrantType.QT_LEFT_TOP:
					grid[0, 0] = q;
					break;
				case QuadrantType.QT_LEFT_BOTTOM:
					grid[1, 0] = q;
					break;
				case QuadrantType.QT_RIGHT_TOP:
					grid[0, 1] = q;
					break;
				case QuadrantType.QT_RIGHT_BOTTOM:
					grid[1, 1] = q;
					break;
				default:
					// ignore QT_ALL or unknown values
					break;
			}
		}

		int compositeWidth = qWidth * 2;
		int compositeHeight = qHeight * 2;

		int outWidth = compositeWidth / Math.Max(1, downsample);
		int outHeight = compositeHeight / Math.Max(1, downsample);

		if (outWidth <= 0 || outHeight <= 0)
		{
			Console.WriteLine("Invalid output dimensions.");
			return false;
		}

		// Respect maxWidth
		if (outWidth > maxWidth)
		{
			int factor = (int)Math.Ceiling((double)outWidth / maxWidth);
			downsample *= Math.Max(1, factor);
			outWidth = compositeWidth / Math.Max(1, downsample);
			outHeight = compositeHeight / Math.Max(1, downsample);
		}

		// Precompute global height min/max (if present) for normalization
		bool hasHeight = false;
		float globalMin = float.MaxValue;
		float globalMax = float.MinValue;
		foreach (var q in Quadrants)
		{
			if (q?.HeightTable == null) continue;
			hasHeight = true;
			var ht = q.HeightTable;
			for (int yy = 0; yy < ht.GetLength(0); yy++)
			{
				for (int xx = 0; xx < ht.GetLength(1); xx++)
				{
					float v = ht[yy, xx];
					if (v < globalMin) globalMin = v;
					if (v > globalMax) globalMax = v;
				}
			}
		}
		if (!hasHeight)
		{
			globalMin = 0f;
			globalMax = 1f;
		}
		float heightRange = Math.Max(1e-6f, globalMax - globalMin);

		try
		{
			using var bmp = new Bitmap(outWidth, outHeight, PixelFormat.Format24bppRgb);

			for (int y = 0; y < outHeight; y++)
			{
				for (int x = 0; x < outWidth; x++)
				{
					int cx = x * downsample;
					int cy = y * downsample;

					int col = cx / qWidth; // 0..1 expected
					int row = cy / qHeight; // 0..1 expected

					// if outside the 2x2 composite, render black
					if (col < 0 || col > 1 || row < 0 || row > 1)
					{
						bmp.SetPixel(x, y, Color.FromArgb(0, 0, 0));
						continue;
					}

					var q = grid[row, col];
					if (q == null || q.NormalTable == null)
					{
						bmp.SetPixel(x, y, Color.FromArgb(0, 0, 0));
						continue;
					}

					int localX = cx % qWidth;
					int localY = cy % qHeight;

					var normal = q.NormalTable;
					if (localY < 0 || localY >= normal.GetLength(0) ||
						localX < 0 || localX >= normal.GetLength(1))
					{
						bmp.SetPixel(x, y, Color.FromArgb(0, 0, 0));
						continue;
					}

					float nx = normal[localY, localX, 0];
					float ny = normal[localY, localX, 1];
					float nz = normal[localY, localX, 2];

					// Map normal components [-1,1] -> [0,1]
					float r = nx * 0.5f + 0.5f;
					float g = ny * 0.5f + 0.5f;
					float b = nz * 0.5f + 0.5f;

					// If lightmap present, modulate brightness
					if (q.LightmapData != null)
					{
						var lm = q.LightmapData;
						if (localY >= 0 && localY < lm.GetLength(0) && localX >= 0 && localX < lm.GetLength(1))
						{
							int lChannels = lm.GetLength(2);
							float lr = lm[localY, localX, 0] / 255f;
							float lg = lChannels > 1 ? lm[localY, localX, 1] / 255f : lr;
							float lb = lChannels > 2 ? lm[localY, localX, 2] / 255f : lr;
							float light = (lr + lg + lb) / 3f;
							float ambient = 0.2f;
							float lightFactor = ambient + 0.8f * light;
							r *= lightFactor;
							g *= lightFactor;
							b *= lightFactor;
						}
					}

					// If height present, adjust brightness slightly based on normalized height
					if (q.HeightTable != null)
					{
						var ht = q.HeightTable;
						if (localY >= 0 && localY < ht.GetLength(0) && localX >= 0 && localX < ht.GetLength(1))
						{
							float h = ht[localY, localX];
							float hn = (h - globalMin) / heightRange; // 0..1
							float heightFactor = 0.7f + 0.6f * hn; // range ~0.7..1.3
							r *= heightFactor;
							g *= heightFactor;
							b *= heightFactor;
						}
					}

					int ri = Math.Clamp((int)(r * 255f), 0, 255);
					int gi = Math.Clamp((int)(g * 255f), 0, 255);
					int bi = Math.Clamp((int)(b * 255f), 0, 255);

					bmp.SetPixel(x, y, Color.FromArgb(ri, gi, bi));
				}
			}

			var dir = Path.GetDirectoryName(outputPath);
			if (!string.IsNullOrEmpty(dir))
				Directory.CreateDirectory(dir);

			bmp.Save(outputPath, ImageFormat.Png);
			return true;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Failed to save bitmap: {ex.Message}");
			return false;
		}
	}
}
