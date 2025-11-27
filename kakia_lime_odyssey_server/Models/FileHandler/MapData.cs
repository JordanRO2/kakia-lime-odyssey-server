using kakia_lime_odyssey_server.Models.FileHandler.Models;
using kakia_lime_odyssey_server.Models.FileHandler.Terrain;
using System.Text.Json;
using System.Drawing;
using System.Drawing.Imaging;

namespace kakia_lime_odyssey_server.Models.FileHandler;

public class MapData
{
	public string Name { get; set; }
	private FullTerrain Terrain { get; set; }
	private FullModels Models { get; set; }

	public NavMesh GeneratedNavMesh { get; set; }

	public MapData(string mapName)
	{
		Name = mapName;
		Terrain = new FullTerrain(Name, GameDataPaths.World.MapsFolder + "/");
		Models = new FullModels(Name, GameDataPaths.World.MapsFolder + "/");


		GeneratedNavMesh = GenerateNavMeshFromTerrainAndModels();
		SaveNavMeshAsJson(GeneratedNavMesh, $"navmeshes/{Name}_navmesh.json");

		SaveNavMeshBitmap(GeneratedNavMesh, $"imgs/{Name}_navmesh.png", 4);
		Terrain.SaveNormalLightHeightBitmap($"imgs/{Name}_normal_light_height.png", 1);
	}

	/// <summary>
	/// Generate a grid-based navmesh by merging FullTerrain.Quadrants (HeightTable / NormalTable)
	/// and FullModels.Quadrants (model bounds). Assumptions:
	/// - Each quadrant's HeightTable is indexed [y,x].
	/// - Each quadrant's CellSize is world units per height cell.
	/// - Model horizontal coordinates are stored in MdiModel.Pos where X = world X, Z = world Y (heightmap Y).
	///   Model vertical coordinate (height) is Pos.Y and is ignored for obstacle projection.
	/// - Models found in a quadrant dictionary entry are considered local to that quadrant; we place them
	///   into the corresponding area of the composed 2x2 height grid before marking blocked cells.
	/// </summary>
	public NavMesh GenerateNavMeshFromTerrainAndModels(double maxSlopeDegrees = 30.0, float agentRadiusWorld = 0.5f)
	{
		// Gather quadrants present in terrain
		var terrainQuads = Terrain.Quadrants ?? new List<ParsedTrnData>();
		if (terrainQuads.Count == 0)
			throw new InvalidOperationException("No terrain quadrants available.");

		// Determine per-quadrant tile sizes (assume consistent sizes across quadrants)
		var sample = terrainQuads.FirstOrDefault(q => q?.HeightTable != null);
		if (sample == null)
			throw new InvalidOperationException("No height tables available in any quadrant.");

		int qHeight = sample.HeightTable.GetLength(0);
		int qWidth = sample.HeightTable.GetLength(1);

		// Compose a 2x2 grid with the same arrangement used by FullTerrain.SaveNormalLightHeightBitmap
		int gw = qWidth * 2;
		int gh = qHeight * 2;

		// Initialize height array and walkable mask (true means potentially walkable)
		var heights = new float[gh, gw];
		var hasHeight = new bool[gh, gw];

		// Determine a canonical cellSize (if different per quadrant, we use the quadrant's value when placing)
		float defaultCellSize = sample.CellSize > 0 ? sample.CellSize : 1f;

		// Map quadrant -> (offsetXCells, offsetYCells)
		(int ox, int oy) GetOffsets(QuadrantType qt)
		{
			return qt switch
			{
				QuadrantType.QT_LEFT_TOP => (0, 0),
				QuadrantType.QT_RIGHT_TOP => (qWidth, 0),
				QuadrantType.QT_LEFT_BOTTOM => (0, qHeight),
				QuadrantType.QT_RIGHT_BOTTOM => (qWidth, qHeight),
				_ => (0, 0)
			};
		}

		// Fill heights from quadrants
		foreach (var q in terrainQuads)
		{
			if (q == null || q.HeightTable == null) continue;
			var (ox, oy) = GetOffsets(q.Quadrant);

			for (int y = 0; y < q.HeightTable.GetLength(0); y++)
			{
				for (int x = 0; x < q.HeightTable.GetLength(1); x++)
				{
					int gx = ox + x;
					int gy = oy + y;
					if (gx < 0 || gx >= gw || gy < 0 || gy >= gh) continue;

					heights[gy, gx] = q.HeightTable[y, x];
					hasHeight[gy, gx] = true;
				}
			}
		}

		// If any cell lacks height, mark as not walkable later.
		// Compute slope per cell using central differences. Assumes uniform world spacing per cell.
		// Use defaultCellSize (assumes cellSize consistent). If quadrants differ, results are approximate.
		double degPerRad = 180.0 / Math.PI;
		var slopeDeg = new double[gh, gw];
		for (int y = 0; y < gh; y++)
		{
			for (int x = 0; x < gw; x++)
			{
				if (!hasHeight[y, x])
				{
					slopeDeg[y, x] = double.PositiveInfinity;
					continue;
				}

				// find neighbors for dx and dy (use nearest valid neighbors)
				int xl = Math.Max(0, x - 1);
				int xr = Math.Min(gw - 1, x + 1);
				int yu = Math.Max(0, y - 1);
				int yd = Math.Min(gh - 1, y + 1);

				if (!hasHeight[y, xl] || !hasHeight[y, xr] || !hasHeight[yu, x] || !hasHeight[yd, x])
				{
					// fallback conservative: if any neighbor missing, mark steep/unwalkable
					slopeDeg[y, x] = double.PositiveInfinity;
					continue;
				}

				double dzdx = (heights[y, xr] - heights[y, xl]) / (2.0 * defaultCellSize);
				double dzdy = (heights[yd, x] - heights[yu, x]) / (2.0 * defaultCellSize);
				double grad = Math.Sqrt(dzdx * dzdx + dzdy * dzdy);
				slopeDeg[y, x] = Math.Atan(grad) * degPerRad;
			}
		}

		// Initially mark walkable by slope and presence of height
		var walkable = new bool[gh, gw];
		for (int y = 0; y < gh; y++)
		{
			for (int x = 0; x < gw; x++)
			{
				walkable[y, x] = hasHeight[y, x] && !double.IsInfinity(slopeDeg[y, x]) && slopeDeg[y, x] <= maxSlopeDegrees;
			}
		}

		// Project models into the grid and block cells under model bounds.
		// For each model in each quadrant, compute global cell index from model center (X,Z).
		// Heuristic: model.Pos.X -> world X, model.Pos.Z -> world Y (heightmap Y axis).
		if (Models?.Quadrants != null)
		{
			foreach (var kv in Models.Quadrants)
			{
				var quadType = kv.Key;
				var modelList = kv.Value;
				var (ox, oy) = GetOffsets(quadType);

				// find quadrant cellSize (if terrain has it)
				var terrainQuad = terrainQuads.FirstOrDefault(q => q.Quadrant == quadType);
				float cellSize = terrainQuad?.CellSize > 0 ? terrainQuad.CellSize : defaultCellSize;

				if (modelList == null) continue;
				foreach (var m in modelList)
				{
					// choose model center: Bound.Center if present else Pos
					Vec3? center = m.Bound?.Center ?? m.Pos;
					if (center == null) continue;

					// Interpret center: X -> world X, Z -> world Y (heightmap index). This is the most common mapping.
					double worldX = center.Value.X;
					double worldY = center.Value.Z;

					// Convert to local cell indices within quadrant
					int localX = (int)Math.Round(worldX / cellSize);
					int localY = (int)Math.Round(worldY / cellSize);

					// Global indices
					int gx = ox + localX;
					int gy = oy + localY;

					// radius
					float radiusWorld = m.Bound?.Radius ?? 0f;
					int radiusCells = Math.Max(1, (int)Math.Ceiling(radiusWorld / cellSize));

					// mark blocked cells in a circle around the center
					for (int yy = gy - radiusCells; yy <= gy + radiusCells; yy++)
					{
						if (yy < 0 || yy >= gh) continue;
						for (int xx = gx - radiusCells; xx <= gx + radiusCells; xx++)
						{
							if (xx < 0 || xx >= gw) continue;
							double dx = (xx - gx) * cellSize;
							double dy = (yy - gy) * cellSize;
							if (dx * dx + dy * dy <= (radiusWorld + 0.5 * cellSize) * (radiusWorld + 0.5 * cellSize))
							{
								walkable[yy, xx] = false;
							}
						}
					}
				}
			}
		}

		// Build NavNodes and neighbor links (4-connectivity)
		var nodes = new List<NavNode>(gw * gh);
		for (int y = 0; y < gh; y++)
		{
			for (int x = 0; x < gw; x++)
			{
				bool w = walkable[y, x];
				float h = hasHeight[y, x] ? heights[y, x] : 0f;
				// world coords for node (X,Z): use cell center coordinate
				float worldX = (x + 0.5f) * defaultCellSize;
				float worldY = (y + 0.5f) * defaultCellSize;
				nodes.Add(new NavNode(x, y, worldX, worldY, h, w));
			}
		}

		// assign neighbors
		for (int y = 0; y < gh; y++)
		{
			for (int x = 0; x < gw; x++)
			{
				int idx = y * gw + x;
				if (!nodes[idx].Walkable) continue;

				var nbrs = new List<int>(4);
				void TryAdd(int nx, int ny)
				{
					if (nx < 0 || nx >= gw || ny < 0 || ny >= gh) return;
					int ni = ny * gw + nx;
					if (nodes[ni].Walkable) nbrs.Add(ni);
				}

				TryAdd(x - 1, y);
				TryAdd(x + 1, y);
				TryAdd(x, y - 1);
				TryAdd(x, y + 1);

				nodes[idx] = nodes[idx].WithNeighbors(nbrs);
			}
		}

		return new NavMesh
		{
			GridWidth = gw,
			GridHeight = gh,
			CellSize = defaultCellSize,
			Nodes = nodes
		};
	}

	public void SaveNavMeshAsJson(NavMesh mesh, string path)
	{
		var opts = new JsonSerializerOptions { WriteIndented = true };
		var json = JsonSerializer.Serialize(mesh, opts);
		var dir = Path.GetDirectoryName(path);
		if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
		File.WriteAllText(path, json);
	}

	/// <summary>
	/// Visualize the navmesh as a bitmap and save to disk.
	/// - Walkable cells are shaded by height.
	/// - Non-walkable cells are colored red.
	/// - Model centers (from FullModels) are overlaid as blue circles.
	/// </summary>
	/// <param name="mesh">NavMesh produced by GenerateNavMeshFromTerrainAndModels</param>
	/// <param name="outputPath">File path to save (png recommended)</param>
	/// <param name="cellPixel">How many pixels per nav cell (scale)</param>
	/// <returns>true on success</returns>
	public bool SaveNavMeshBitmap(NavMesh mesh, string outputPath, int cellPixel = 4)
	{
		if (mesh == null) throw new ArgumentNullException(nameof(mesh));
		if (cellPixel <= 0) cellPixel = 4;

		int imgW = mesh.GridWidth * cellPixel;
		int imgH = mesh.GridHeight * cellPixel;
		if (imgW <= 0 || imgH <= 0) return false;

		// compute height range
		float minH = float.MaxValue, maxH = float.MinValue;
		foreach (var n in mesh.Nodes)
		{
			if (n.Height < minH) minH = n.Height;
			if (n.Height > maxH) maxH = n.Height;
		}
		if (minH == float.MaxValue) { minH = 0; maxH = 1; }
		float range = Math.Max(1e-6f, maxH - minH);

		try
		{
			using var bmp = new Bitmap(imgW, imgH, PixelFormat.Format24bppRgb);
			using var g = Graphics.FromImage(bmp);
			g.Clear(Color.Black);

			// Draw cells
			for (int y = 0; y < mesh.GridHeight; y++)
			{
				for (int x = 0; x < mesh.GridWidth; x++)
				{
					int idx = y * mesh.GridWidth + x;
					var node = mesh.Nodes[idx];
					int sx = x * cellPixel;
					int sy = y * cellPixel;
					Rectangle rect = new Rectangle(sx, sy, cellPixel, cellPixel);

					if (!node.Walkable)
					{
						// blocked = red
						using var br = new SolidBrush(Color.FromArgb(200, 40, 40));
						g.FillRectangle(br, rect);
					}
					else
					{
						// shade by height (blue low -> green -> yellow high)
						float hn = (node.Height - minH) / range;
						hn = Math.Clamp(hn, 0f, 1f);
						Color c;
						if (hn < 0.5f)
						{
							// blue -> green
							int r = (int)(hn * 2 * 60);
							int gcol = (int)(hn * 2 * 180);
							int bcol = 200 - (int)(hn * 2 * 60);
							c = Color.FromArgb(Math.Clamp(r, 0, 255), Math.Clamp(gcol, 0, 255), Math.Clamp(bcol, 0, 255));
						}
						else
						{
							// green -> yellow
							float t = (hn - 0.5f) * 2f;
							int r = (int)(t * 220);
							int gcol = 180;
							int bcol = (int)((1 - t) * 60);
							c = Color.FromArgb(Math.Clamp(r, 0, 255), Math.Clamp(gcol, 0, 255), Math.Clamp(bcol, 0, 255));
						}
						using var br = new SolidBrush(c);
						g.FillRectangle(br, rect);
					}
				}
			}

			// Draw grid lines (optional subtle)
			using (var pen = new Pen(Color.FromArgb(40, Color.Black)))
			{
				for (int x = 0; x <= mesh.GridWidth; x++)
				{
					int px = x * cellPixel;
					g.DrawLine(pen, px, 0, px, imgH);
				}
				for (int y = 0; y <= mesh.GridHeight; y++)
				{
					int py = y * cellPixel;
					g.DrawLine(pen, 0, py, imgW, py);
				}
			}

			// Overlay model centers (blue) and main bounds (white) if available
			if (Models?.Quadrants != null)
			{
				using var modelBrush = new SolidBrush(Color.FromArgb(220, 60, 140, 255));
				using var modelPen = new Pen(Color.FromArgb(220, 30, 90, 200), 1f);
				foreach (var kv in Models.Quadrants)
				{
					var modelList = kv.Value;
					if (modelList == null) continue;
					foreach (var m in modelList)
					{
						Vec3? center = m.Bound?.Center ?? m.Pos;
						if (center == null) continue;

						// map model world to grid index
						double worldX = center.Value.X;
						double worldY = center.Value.Z;
						int gx = (int)Math.Round(worldX / mesh.CellSize);
						int gy = (int)Math.Round(worldY / mesh.CellSize);

						if (gx < 0 || gx >= mesh.GridWidth || gy < 0 || gy >= mesh.GridHeight) continue;

						int cx = gx * cellPixel + cellPixel / 2;
						int cy = gy * cellPixel + cellPixel / 2;
						int radPx = Math.Max(1, (int)Math.Ceiling((m.Bound?.Radius ?? 0f) / mesh.CellSize) * cellPixel);

						Rectangle r = new Rectangle(cx - radPx, cy - radPx, radPx * 2, radPx * 2);
						g.FillEllipse(modelBrush, r);
						g.DrawEllipse(modelPen, r);
					}
				}
			}

			var dir = Path.GetDirectoryName(outputPath);
			if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
			bmp.Save(outputPath, ImageFormat.Png);
			return true;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Failed to save navmesh bitmap: {ex.Message}");
			return false;
		}
	}


	// Navmesh types
	public record NavMesh
	{
		public int GridWidth { get; init; }
		public int GridHeight { get; init; }
		/// <summary>World size of each grid cell (meters / game units).</summary>
		public float CellSize { get; init; }
		public List<NavNode> Nodes { get; init; } = new();
	}

	public record NavNode
	{
		public int GridX { get; init; }
		public int GridY { get; init; }
		public float WorldX { get; init; }
		public float WorldY { get; init; }
		public float Height { get; init; }
		public bool Walkable { get; init; }
		public List<int> Neighbors { get; init; } = new();

		public NavNode(int gx, int gy, float wx, float wy, float h, bool walkable)
		{
			GridX = gx;
			GridY = gy;
			WorldX = wx;
			WorldY = wy;
			Height = h;
			Walkable = walkable;
			Neighbors = new List<int>();
		}

		public NavNode WithNeighbors(List<int> neighbors) =>
			this with { Neighbors = neighbors };
	}
}
