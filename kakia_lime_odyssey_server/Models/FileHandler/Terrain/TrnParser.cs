using System.Numerics;
using System.Text;

namespace kakia_lime_odyssey_server.Models.FileHandler.Terrain;


public class TrnParser
{
	private readonly string _filePath;
	private readonly QuadrantType _quadrantType;

	// This property holds all the parsed data
	public ParsedTrnData Data { get; private set; }

	// --- File-derived values ---
	private int _versionMajor;
	private int _versionMinor;
	private float _mCellSize;
	private int _mTerrainShiftFile;

	// --- Calculated values (from InitSize) ---
	private int _mCalcShift;
	private int _mTableWidth;
	private int _mNumXHeights;
	private int _mNumSectorX;
	private int _mNumSectorY;
	private int _mLightmapTexelPerCell;

	/// <summary>
	/// Initializes the parser.
	/// </summary>
	/// <param name="filePath">Path to the .trn file.</param>
	/// <param name="quadrantType">The quadrant this file represents.</param>
	public TrnParser(string filePath, QuadrantType quadrantType)
	{
		_filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
		_quadrantType = quadrantType;
		Data = new ParsedTrnData();
	}

	/// <summary>
	/// Helper to check file version.
	/// </summary>
	private bool CheckVersion(int major, int minor)
	{
		if (_versionMajor > major) return true;
		if (_versionMajor == major && _versionMinor >= minor) return true;
		return false;
	}

	/// <summary>
	/// Port of the C++ 'InitSize' function.
	/// It calculates all the critical dimensions for the terrain.
	/// </summary>
	private void InitSize()
	{
		_mCalcShift = _mTerrainShiftFile;
		if (_quadrantType != QuadrantType.QT_ALL)
		{
			_mCalcShift -= 1;
		}
		Console.WriteLine($"Using calculated terrain shift: {_mCalcShift}");

		_mTableWidth = 1 << _mCalcShift;
		_mNumXHeights = _mTableWidth + 1;
		_mNumSectorX = _mTableWidth >> 3;
		_mNumSectorY = _mTableWidth >> 3;

		Console.WriteLine("--- Calculated Sizes (from C++) ---");
		Console.WriteLine($"  m_tableWidth (cells): {_mTableWidth}");
		Console.WriteLine($"  m_numXHeights (verts): {_mNumXHeights}");
		Console.WriteLine($"  m_numSectorX: {_mNumSectorX}");
		Console.WriteLine($"  m_numSectorY: {_mNumSectorY}");
		Console.WriteLine("----------------------------------");
	}

	// --- BLOCK PARSERS (I/O-BOUND) ---

	/// <summary>
	/// Reads the strided heightmap data (float) from the file.
	/// </summary>
	private void ReadHeightTable(BinaryReader reader)
	{
		Console.WriteLine("--- Reading HeightTable (strided) ---");
		int nHeightTableLen = _mNumXHeights;
		var heightTable = new float[nHeightTableLen, nHeightTableLen];

		int oHeightTableLen = (_quadrantType == QuadrantType.QT_ALL)
			? nHeightTableLen
			: (2 * _mTableWidth) + 1;

		int lenDiff = oHeightTableLen - nHeightTableLen;
		const int itemSize = 4; // 4 bytes per float
		long bytesToSkipInRow = (long)lenDiff * itemSize;
		long bytesPerFullRow = (long)oHeightTableLen * itemSize;

		for (int y = 0; y < oHeightTableLen; y++)
		{
			int localY;
			bool skipFullRow = false;
			bool readData = true;
			bool seekAtStart = false;

			switch (_quadrantType)
			{
				case QuadrantType.QT_LEFT_TOP:
					if (y >= nHeightTableLen) skipFullRow = true;
					localY = y;
					break;
				case QuadrantType.QT_LEFT_BOTTOM:
					if (y < nHeightTableLen - 1) skipFullRow = true;
					localY = y - (nHeightTableLen - 1);
					break;
				case QuadrantType.QT_RIGHT_TOP:
					if (y >= nHeightTableLen) skipFullRow = true;
					localY = y;
					seekAtStart = true;
					break;
				case QuadrantType.QT_RIGHT_BOTTOM:
					if (y < nHeightTableLen - 1) skipFullRow = true;
					localY = y - (nHeightTableLen - 1);
					seekAtStart = true;
					break;
				case QuadrantType.QT_ALL:
				default:
					localY = y;
					break;
			}

			if (skipFullRow)
			{
				reader.BaseStream.Seek(bytesPerFullRow, SeekOrigin.Current);
				continue;
			}

			if (seekAtStart)
			{
				reader.BaseStream.Seek(bytesToSkipInRow, SeekOrigin.Current);
			}

			for (int x = 0; x < nHeightTableLen; x++)
			{
				heightTable[localY, x] = reader.ReadSingle();
			}

			if (!seekAtStart && _quadrantType != QuadrantType.QT_ALL)
			{
				reader.BaseStream.Seek(bytesToSkipInRow, SeekOrigin.Current);
			}
		}

		Console.WriteLine($"Read HeightTable, shape=[{heightTable.GetLength(0)}, {heightTable.GetLength(1)}]");
		Data.HeightTable = heightTable;
	}

	/// <summary>
	/// Reads the strided normalmap data (3x float) from the file.
	/// </summary>
	private void ReadNormalTable(BinaryReader reader)
	{
		Console.WriteLine("--- Reading NormalTable (strided) ---");
		int nHeightTableLen = _mNumXHeights;
		var normalTable = new float[nHeightTableLen, nHeightTableLen, 3];

		int oHeightTableLen = (_quadrantType == QuadrantType.QT_ALL)
			? nHeightTableLen
			: (2 * _mTableWidth) + 1;

		int lenDiff = oHeightTableLen - nHeightTableLen;
		const int itemSize = 12; // 12 bytes per NiPoint3 (3 * float)
		long bytesToSkipInRow = (long)lenDiff * itemSize;
		long bytesPerFullRow = (long)oHeightTableLen * itemSize;

		for (int y = 0; y < oHeightTableLen; y++)
		{
			int localY;
			bool skipFullRow = false;
			bool seekAtStart = false;

			switch (_quadrantType)
			{
				case QuadrantType.QT_LEFT_TOP:
					if (y >= nHeightTableLen) skipFullRow = true;
					localY = y;
					break;
				case QuadrantType.QT_LEFT_BOTTOM:
					if (y < nHeightTableLen - 1) skipFullRow = true;
					localY = y - (nHeightTableLen - 1);
					break;
				case QuadrantType.QT_RIGHT_TOP:
					if (y >= nHeightTableLen) skipFullRow = true;
					localY = y;
					seekAtStart = true;
					break;
				case QuadrantType.QT_RIGHT_BOTTOM:
					if (y < nHeightTableLen - 1) skipFullRow = true;
					localY = y - (nHeightTableLen - 1);
					seekAtStart = true;
					break;
				case QuadrantType.QT_ALL:
				default:
					localY = y;
					break;
			}

			if (skipFullRow)
			{
				reader.BaseStream.Seek(bytesPerFullRow, SeekOrigin.Current);
				continue;
			}

			if (seekAtStart)
			{
				reader.BaseStream.Seek(bytesToSkipInRow, SeekOrigin.Current);
			}

			for (int x = 0; x < nHeightTableLen; x++)
			{
				normalTable[localY, x, 0] = reader.ReadSingle(); // X
				normalTable[localY, x, 1] = reader.ReadSingle(); // Y
				normalTable[localY, x, 2] = reader.ReadSingle(); // Z
			}

			if (!seekAtStart && _quadrantType != QuadrantType.QT_ALL)
			{
				reader.BaseStream.Seek(bytesToSkipInRow, SeekOrigin.Current);
			}
		}
		Console.WriteLine($"Read NormalTable, shape=[{normalTable.GetLength(0)}, {normalTable.GetLength(1)}, 3]");
		Data.NormalTable = normalTable;
	}

	/// <summary>
	/// Reads the strided splat map data (unsigned int) from the file.
	/// </summary>
	private uint[,] ReadMixPixel(BinaryReader reader)
	{
		Console.WriteLine("--- Reading MixPixel (strided) ---");
		int nPixelLen = 4 * _mTableWidth;
		var mixPixelTable = new uint[nPixelLen, nPixelLen];

		int oPixelLen = (_quadrantType == QuadrantType.QT_ALL)
			? nPixelLen
			: 2 * nPixelLen;

		int lenDiff = oPixelLen - nPixelLen;
		const int itemSize = 4; // 4 bytes per uint
		long bytesToSkipInRow = (long)lenDiff * itemSize;
		long bytesPerFullRow = (long)oPixelLen * itemSize;

		for (int y = 0; y < oPixelLen; y++)
		{
			int localY;
			bool skipFullRow = false;
			bool seekAtStart = false;

			switch (_quadrantType)
			{
				case QuadrantType.QT_LEFT_TOP:
					if (y >= nPixelLen) skipFullRow = true;
					localY = y;
					break;
				case QuadrantType.QT_LEFT_BOTTOM:
					if (y < nPixelLen) skipFullRow = true;
					localY = y - nPixelLen;
					break;
				case QuadrantType.QT_RIGHT_TOP:
					if (y >= nPixelLen) skipFullRow = true;
					localY = y;
					seekAtStart = true;
					break;
				case QuadrantType.QT_RIGHT_BOTTOM:
					if (y < nPixelLen) skipFullRow = true;
					localY = y - nPixelLen;
					seekAtStart = true;
					break;
				case QuadrantType.QT_ALL:
				default:
					localY = y;
					break;
			}

			if (skipFullRow)
			{
				reader.BaseStream.Seek(bytesPerFullRow, SeekOrigin.Current);
				continue;
			}

			if (seekAtStart)
			{
				reader.BaseStream.Seek(bytesToSkipInRow, SeekOrigin.Current);
			}

			for (int x = 0; x < nPixelLen; x++)
			{
				mixPixelTable[localY, x] = reader.ReadUInt32();
			}

			if (!seekAtStart && _quadrantType != QuadrantType.QT_ALL)
			{
				reader.BaseStream.Seek(bytesToSkipInRow, SeekOrigin.Current);
			}
		}

		Console.WriteLine($"Read MixPixel, shape=[{mixPixelTable.GetLength(0)}, {mixPixelTable.GetLength(1)}]");
		return mixPixelTable;
	}

	/// <summary>
	/// Reads the strided lightmap data (RGBA32) from the file.
	/// </summary>
	private void ReadLightmapData(BinaryReader reader)
	{
		Console.WriteLine("--- Reading Lightmap (strided) ---");

		int oTextureWidth, oTextureHeight;
		int nTextureWidth, nTextureHeight;

		if (_quadrantType == QuadrantType.QT_ALL)
		{
			oTextureWidth = _mNumSectorX * 8 * _mLightmapTexelPerCell;
			oTextureHeight = _mNumSectorY * 8 * _mLightmapTexelPerCell;
			nTextureWidth = oTextureWidth;
			nTextureHeight = oTextureHeight;
		}
		else
		{
			oTextureWidth = (2 * _mNumSectorX) * 8 * _mLightmapTexelPerCell;
			oTextureHeight = (2 * _mNumSectorY) * 8 * _mLightmapTexelPerCell;
			nTextureWidth = _mNumSectorX * 8 * _mLightmapTexelPerCell;
			nTextureHeight = _mNumSectorY * 8 * _mLightmapTexelPerCell;
		}

		Console.WriteLine($"  Lightmap local size: {nTextureWidth}x{nTextureHeight}");
		Console.WriteLine($"  Lightmap file size: {oTextureWidth}x{oTextureHeight}");

		var lightmapTable = new byte[nTextureHeight, nTextureWidth, 4];
		const int itemSize = 4; // 4 bytes for RGBA32
		long bytesPerLocalRow = (long)nTextureWidth * itemSize;
		long bytesToSkipInRow = bytesPerLocalRow;
		long bytesPerFullRow = (long)oTextureWidth * itemSize;

		for (int y = 0; y < oTextureHeight; y++)
		{
			int localY;
			bool skipFullRow = false;
			bool seekAtStart = false;

			switch (_quadrantType)
			{
				case QuadrantType.QT_LEFT_TOP:
					if (y >= nTextureHeight) skipFullRow = true;
					localY = y;
					break;
				case QuadrantType.QT_LEFT_BOTTOM:
					if (y < nTextureHeight) skipFullRow = true;
					localY = y - nTextureHeight;
					break;
				case QuadrantType.QT_RIGHT_TOP:
					if (y >= nTextureHeight) skipFullRow = true;
					localY = y;
					seekAtStart = true;
					break;
				case QuadrantType.QT_RIGHT_BOTTOM:
					if (y < nTextureHeight) skipFullRow = true;
					localY = y - nTextureHeight;
					seekAtStart = true;
					break;
				case QuadrantType.QT_ALL:
				default:
					localY = y;
					break;
			}

			if (skipFullRow)
			{
				reader.BaseStream.Seek(bytesPerFullRow, SeekOrigin.Current);
				continue;
			}

			if (seekAtStart)
			{
				reader.BaseStream.Seek(bytesToSkipInRow, SeekOrigin.Current);
			}

			for (int x = 0; x < nTextureWidth; x++)
			{
				lightmapTable[localY, x, 0] = reader.ReadByte(); // R
				lightmapTable[localY, x, 1] = reader.ReadByte(); // G
				lightmapTable[localY, x, 2] = reader.ReadByte(); // B
				lightmapTable[localY, x, 3] = reader.ReadByte(); // A
			}

			if (!seekAtStart && _quadrantType != QuadrantType.QT_ALL)
			{
				reader.BaseStream.Seek(bytesToSkipInRow, SeekOrigin.Current);
			}
		}

		Console.WriteLine($"Read Lightmap, shape=[{lightmapTable.GetLength(0)}, {lightmapTable.GetLength(1)}, 4]");
		Data.LightmapData = lightmapTable;
	}

	/// <summary>
	/// Reads the texture layer names.
	/// </summary>
	private void ReadLayerTextureBackground_1_8(BinaryReader reader)
	{
		Console.WriteLine("--- Reading LayerTextureBackground ---");
		Data.LayerTextureNames = new List<string>();

		int bSmoothLayering = reader.ReadInt32();

		if (bSmoothLayering != 0)
		{
			Console.WriteLine("  Smooth layering mode: Reading 8 strings");
			for (int i = 0; i < 8; i++)
			{
				int nameLen = reader.ReadInt32();
				if (nameLen >= 1024) Console.WriteLine($"Warning: String length {nameLen} >= 1024");
				byte[] nameData = reader.ReadBytes(nameLen);
				Data.LayerTextureNames.Add(Encoding.ASCII.GetString(nameData));
			}
		}
		else
		{
			Console.WriteLine("  Legacy layering mode: Reading strings for all sectors");
			int numSectors = (2 * _mNumSectorY) * (2 * _mNumSectorX);
			for (int i = 0; i < numSectors; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					int nameLen = reader.ReadInt32();
					if (nameLen >= 1024) Console.WriteLine($"Warning: String length {nameLen} >= 1024");
					byte[] nameData = reader.ReadBytes(nameLen);

					if (i == 0) // Only store the first set
					{
						Data.LayerTextureNames.Add(Encoding.ASCII.GetString(nameData));
					}
				}
			}
		}
		Console.WriteLine($"Read {Data.LayerTextureNames.Count} texture names.");
	}

	/// <summary>
	/// Reads the .trn file based on the C++ code's structure.
	/// </summary>
	public ParsedTrnData Parse()
	{
		Console.WriteLine($"Parsing '{_filePath}' as Quadrant {_quadrantType}");

		using (var fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read))
		using (var reader = new BinaryReader(fs, Encoding.ASCII))
		{
			// --- Header (from LoadThread) ---
			Data.Magic = Encoding.ASCII.GetString(reader.ReadBytes(7));
			if (Data.Magic != "Terrain")
			{
				throw new InvalidDataException($"Not a terrain file. Magic: {Data.Magic}");
			}
			Console.WriteLine($"Magic: {Data.Magic}");

			_versionMajor = reader.ReadInt32();
			_versionMinor = reader.ReadInt32();
			Data.Version = (_versionMajor, _versionMinor);
			Console.WriteLine($"Version: {_versionMajor}.{_versionMinor}");

			_mCellSize = reader.ReadSingle();
			Data.CellSize = _mCellSize;
			Console.WriteLine($"Cell Size: {_mCellSize}");

			_mTerrainShiftFile = reader.ReadInt32();
			Data.TerrainShiftFile = _mTerrainShiftFile;
			Console.WriteLine($"Terrain Shift (from file): {_mTerrainShiftFile}");

			InitSize();

			// --- Height Table Block ---
			reader.BaseStream.Seek(4, SeekOrigin.Current); // ZFile::Seek(&file, 4);
			Console.WriteLine("Seek 4 bytes (before HeightTable)");
			ReadHeightTable(reader);

			// --- Normal Table Block ---
			reader.BaseStream.Seek(4, SeekOrigin.Current); // ZFile::Seek(&file, 4);
			Console.WriteLine("Seek 4 bytes (before NormalTable)");
			ReadNormalTable(reader);

			// --- Mix Pixels (Splat Maps) Block ---
			if (CheckVersion(1, 7))
			{
				Data.MixPixels1 = ReadMixPixel(reader);
				if (CheckVersion(1, 8))
				{
					Data.MixPixels2 = ReadMixPixel(reader);
				}
			}
			else
			{
				Console.WriteLine("Skipping MixPixel blocks (version < 1.7)");
			}

			// --- Lightmap Texture Block ---
			if (CheckVersion(2, 2))
			{
				_mLightmapTexelPerCell = reader.ReadInt32();
				Data.LightmapTexelPerCell = _mLightmapTexelPerCell;
				Console.WriteLine($"Lightmap Texel/Cell: {_mLightmapTexelPerCell}");
				ReadLightmapData(reader);
			}
			else
			{
				Console.WriteLine("Skipping Lightmap block (version < 2.2)");
			}

			// --- Punch Info Block ---
			int oNumSectorX, oNumSectorY;
			if (_quadrantType == QuadrantType.QT_ALL)
			{
				oNumSectorX = _mNumSectorX;
				oNumSectorY = _mNumSectorY;
			}
			else
			{
				oNumSectorX = 2 * _mNumSectorX;
				oNumSectorY = 2 * _mNumSectorY;
			}
			int numSectorsTotal = oNumSectorX * oNumSectorY;

			if (CheckVersion(1, 9))
			{
				Console.WriteLine($"Reading Punch Info for {numSectorsTotal} sectors");
				Data.PunchInfo = new List<(int, byte[]?)>(numSectorsTotal);
				for (int i = 0; i < numSectorsTotal; i++)
				{
					int bPunched = reader.ReadInt32();
					byte[]? punchData = null;
					if (bPunched != 0)
					{
						punchData = reader.ReadBytes(8);
					}
					Data.PunchInfo.Add((bPunched, punchData));
				}
			}
			else
			{
				Console.WriteLine("Skipping Punch Info block (version < 1.9)");
			}

			// --- Main Bound Block ---
			if (CheckVersion(1, 2))
			{
				var center = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
				float radius = reader.ReadSingle();
				Data.MainBound = (center, radius);
				Console.WriteLine($"Read Main Bound: {Data.MainBound}");
			}
			else
			{
				Console.WriteLine("Skipping Main Bound block (version < 1.2)");
			}

			// --- Sector Bounds Block ---
			if (CheckVersion(1, 3))
			{
				int sectorBoundsSize = numSectorsTotal * 16;
				Console.WriteLine($"Reading Sector Bounds block ({sectorBoundsSize} bytes)");
				Data.SectorBoundsData = reader.ReadBytes(sectorBoundsSize);
			}
			else
			{
				Console.WriteLine("Skipping Sector Bounds block (version < 1.3)");
			}

			// --- Sector Layer Info Block ---
			if (CheckVersion(2, 0))
			{
				int sectorLayerSize = numSectorsTotal * 12;
				Console.WriteLine($"Reading Sector Layer block ({sectorLayerSize} bytes)");
				Data.SectorLayerData = reader.ReadBytes(sectorLayerSize);
			}
			else
			{
				Console.WriteLine("Skipping Sector Layer block (version < 2.0)");
			}

			// --- Layer Textures Block ---
			if (CheckVersion(1, 8))
			{
				ReadLayerTextureBackground_1_8(reader);
			}

			// --- End of Parse ---
			Console.WriteLine("\nParsing complete.");
			long pos = reader.BaseStream.Position;
			long endPos = reader.BaseStream.Length;

			Console.WriteLine($"File pointer at: {pos} / {endPos} bytes");
			if (pos != endPos)
			{
				Console.WriteLine($"WARNING: {endPos - pos} unparsed bytes remain.");
			}

			return Data;
		}
	}
}
