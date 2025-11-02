using System.Text;

namespace kakia_lime_odyssey_server.Models.FileHandler.Models;

public class MdiParser
{
    private readonly string _filePath;

    // Backing dictionary, "models" will reference Models list.
    public Dictionary<string, object> Data { get; } = new();
    public List<MdiModel> Models { get; } = new();

    public int VersionMajor { get; private set; }
    public int VersionMinor { get; private set; }

    public MdiParser(string filePath)
    {
        _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
    }

    private string ReadLengthPrefixedString(BinaryReader br)
    {
        // Read single-byte length prefix
        int nameLen;
        try
        {
            nameLen = br.ReadByte();
        }
        catch (EndOfStreamException)
        {
            throw new EndOfStreamException("File ended unexpectedly while reading string length.");
        }

        if (nameLen > 0)
        {
            if (nameLen >= 128)
                Console.WriteLine($"Warning: String length {nameLen} >= 128");

            var bytes = br.ReadBytes(nameLen);
            if (bytes.Length != nameLen)
                throw new EndOfStreamException("File ended unexpectedly while reading string data.");

            // ASCII decode; fallback to hex representation if bytes are not mappable
            try
            {
                return Encoding.ASCII.GetString(bytes);
            }
            catch
            {
                return BitConverter.ToString(bytes);
            }
        }

        return string.Empty;
    }

    private void ReadModelsV1_8(BinaryReader br, int numModel)
    {
        Console.WriteLine($"Reading {numModel} models (v1.8+ format)...");
        for (int i = 0; i < numModel; i++)
        {
            var model = new MdiModel
            {
                Name = ReadLengthPrefixedString(br),
                ResName = ReadLengthPrefixedString(br),
                RelativeDir = ReadLengthPrefixedString(br),
                TextureChangeInfoFile = ReadLengthPrefixedString(br)
            };

            // Expect 14 floats (56 bytes)
            float[] data = new float[14];
            for (int j = 0; j < 14; j++)
            {
                try
                {
                    data[j] = br.ReadSingle();
                }
                catch (EndOfStreamException)
                {
                    throw new EndOfStreamException($"File ended unexpectedly reading model {i} data block.");
                }
            }

            model.Pos = new Vec3(data[0], data[1], data[2]);
            model.Rot = new Vec3(data[3], data[4], data[5]);
            model.Scale = new Vec3(data[6], data[7], data[8]);
            model.Bound = new Bound
            {
                Center = new Vec3(data[9], data[10], data[11]),
                Radius = data[12]
            };
            model.Water = (int)data[13];

            Models.Add(model);
        }

        Data["models"] = Models;
    }

    private void ReadModelsLegacy(BinaryReader br, int numModel, int structSize)
    {
        Console.WriteLine($"Reading {numModel} legacy models ({structSize} bytes each)...");
        long totalSize = (long)numModel * structSize;

        if (totalSize > int.MaxValue)
            throw new InvalidOperationException("Requested legacy model block is too large to read into memory.");

        var bulk = br.ReadBytes((int)totalSize);
        if (bulk.Length != totalSize)
            throw new EndOfStreamException("File ended unexpectedly reading legacy model block.");

        for (int i = 0; i < numModel; i++)
        {
            int offset = i * structSize;
            var raw = new byte[structSize];
            Array.Copy(bulk, offset, raw, 0, structSize);

            // Store legacy models as MdiModel instances with RawData populated.
            Models.Add(new MdiModel { RawData = raw });
        }

        Data["models"] = Models;
        Console.WriteLine($"Warning: Read {numModel} legacy models as raw {structSize}-byte blocks.");
    }

    public Dictionary<string, object> Parse()
    {
        Console.WriteLine($"--- [MDI] Parsing '{_filePath}' ---");

        using var fs = File.OpenRead(_filePath);
        using var br = new BinaryReader(fs, Encoding.ASCII, leaveOpen: false);

        // Header: 3-byte magic
        var magicBytes = br.ReadBytes(3);
        if (magicBytes.Length != 3)
            throw new EndOfStreamException("File too short to contain magic header.");

        string magic = Encoding.ASCII.GetString(magicBytes);
        Data["magic"] = magic;
        if (!string.Equals(magic, "Mdi", StringComparison.Ordinal))
            throw new InvalidDataException($"Not a Mdi file. Magic: {magic}");
        Console.WriteLine($"Magic: {magic}");

        try
        {
            VersionMajor = br.ReadInt32();
            VersionMinor = br.ReadInt32();
        }
        catch (EndOfStreamException)
        {
            throw new EndOfStreamException("File ended unexpectedly while reading version.");
        }

        Data["version"] = new int[] { VersionMajor, VersionMinor };
        Console.WriteLine($"Version: {VersionMajor}.{VersionMinor}");

        int numModel;
        try
        {
            numModel = br.ReadInt32();
        }
        catch (EndOfStreamException)
        {
            throw new EndOfStreamException("File ended unexpectedly while reading model count.");
        }

        Data["num_model"] = numModel;
        Console.WriteLine($"Model Count: {numModel}");

        if (numModel == 0)
        {
            Console.WriteLine("File contains 0 models. Parsing complete.");
            return Data;
        }

        // Version-dependent parsing
        var vMajor = VersionMajor;
        var vMinor = VersionMinor;

        if ((vMajor == 2 && vMinor == 0) ||
            (vMajor == 1 && vMinor == 9) ||
            (vMajor == 1 && vMinor == 8))
        {
            ReadModelsV1_8(br, numModel);
        }
        else if (vMajor == 1 && vMinor == 7)
        {
            ReadModelsLegacy(br, numModel, 584);
        }
        else if ((vMajor == 1 && vMinor == 6) ||
                 (vMajor == 1 && vMinor == 5) ||
                 (vMajor == 1 && vMinor == 4) ||
                 (vMajor == 1 && vMinor == 2))
        {
            ReadModelsLegacy(br, numModel, 580);
        }
        else if (vMajor == 1 && vMinor == 3)
        {
            ReadModelsLegacy(br, numModel, 588);
        }
        else if (vMajor == 1 && vMinor == 1)
        {
            ReadModelsLegacy(br, numModel, 576);
        }
        else
        {
            ReadModelsLegacy(br, numModel, 572);
        }

        Console.WriteLine("\n[MDI] Parsing complete.");
        long pos = fs.Position;
        long endPos = fs.Length;
        Console.WriteLine($"File pointer at: {pos} / {endPos} bytes");
        if (pos != endPos)
            Console.WriteLine($"WARNING: {endPos - pos} unparsed bytes remain.");

        return Data;
    }
}