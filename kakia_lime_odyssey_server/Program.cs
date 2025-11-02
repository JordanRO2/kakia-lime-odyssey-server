using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_server.Models;
using kakia_lime_odyssey_server.Models.FileHandler;
using kakia_lime_odyssey_server.Models.FileHandler.Terrain;
using kakia_lime_odyssey_server.Network;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
Logger.SetLogLevel(LogLevel.Debug);

/* 
 * Used for generating navmeshes (as well as testing it by creating navmesh images and height/lightmap images)
 * 

var mapNames = Directory.GetFiles("db/maps/", "*.trn*")
	.Select(Path.GetFileName)
	.Select(fn => Regex.Replace(fn, @"\.trn.*$", "", RegexOptions.IgnoreCase))
	.Distinct()
	.OrderBy(n => n)
	.ToList();

foreach (var mapName in mapNames)
{
	MapData map = new MapData(mapName);
}
*/



Config cfg;
if (!File.Exists("config.json"))
{
	cfg = new Config()
	{
		ServerIP = "127.0.0.1",
		Port = 9676,
		Crypto = true
	};
	var json = JsonConvert.SerializeObject(cfg, Formatting.Indented);
	File.WriteAllText("config.json", json);
	Logger.Log("Default config.json generated, if you do not wish to run with the default values then please open it and adjust it to your liking.", LogLevel.Information);
}
else
{
	try
	{
		cfg = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"))!;
	}
	catch (Exception ex) 
	{
		Logger.Log(ex);
		Logger.Log("Failed to load config file, probably malformed config.json", LogLevel.Exception);
		return;
	}
}


Logger.Log("===================================================================================================");
Logger.Log("                         Welcome to the Kakia LimeOdyssey Server!");
Logger.Log("Please note, this server is currently intended to be used with the Korean CBT3 client (rev 211).");
Logger.Log("===================================================================================================");


kakia_lime_odyssey_network.Handler.PacketHandlers.LoadPacketHandlers("kakia_lime_odyssey_server.PacketHandlers");
LimeServer server = new(cfg);
CancellationTokenSource ct = new();

_ = server.Run(ct.Token);

Logger.Log("==== [Server is now running, press Q to quit gracefully] ====", LogLevel.Information);
while (Console.ReadKey().KeyChar.ToString().ToLower() != "q")
{
	await Task.Delay(1000);
}
server.Stop();
ct.Cancel();







/// <summary>
/// Render a simple ASCII visualization of the normal-map stored in ParsedTrnData.NormalTable.
/// - Maps the normal's Z component to a brightness ramp by default.
/// - If the normal is mostly horizontal (low Z), a directional character is used based on X/Y.
/// - Downsampling and a max console width cap are used so the output is readable.
/// </summary>
static void VisualizeNormalMap(ParsedTrnData? data, int downsample = 2, int maxWidth = 120)
{
	if (data?.NormalTable == null)
	{
		Console.WriteLine("No normal table present to visualize.");
		return;
	}

	var normals = data.NormalTable;
	int h = normals.GetLength(0); // y
	int w = normals.GetLength(1); // x
	int comps = normals.GetLength(2);
	if (comps < 3)
	{
		Console.WriteLine("Normal table does not contain 3 components per texel.");
		return;
	}

	// Ensure reasonable steps so output fits in console
	downsample = Math.Max(1, downsample);
	int scaleX = Math.Max(1, (int)Math.Ceiling((double)w / Math.Max(1, maxWidth)));
	int stepX = downsample * scaleX;
	int stepY = downsample;

	// Brightness ramp: low -> high
	const string ramp = " .:-=+*#%@";

	for (int y = 0; y < h; y += stepY)
	{
		var sb = new System.Text.StringBuilder(Math.Max(16, w / stepX));
		for (int x = 0; x < w; x += stepX)
		{
			float nx = normals[y, x, 0];
			float ny = normals[y, x, 1];
			float nz = normals[y, x, 2];

			// Normalize (safeguard)
			float len = (float)Math.Sqrt(nx * nx + ny * ny + nz * nz);
			if (len > 1e-6f)
			{
				nx /= len; ny /= len; nz /= len;
			}
			else
			{
				// Unknown/missing -> display a dot
				sb.Append('.');
				continue;
			}

			// If the normal points mostly up, map Z to brightness
			if (nz >= 0.4f)
			{
				// Map nz from [-1..1] to [0..1]
				float b = (nz + 1f) * 0.5f;
				int idx = (int)Math.Clamp((int)Math.Floor(b * (ramp.Length - 1)), 0, ramp.Length - 1);
				sb.Append(ramp[idx]);
			}
			else
			{
				// Mostly horizontal: show direction based on XY
				// Use simple ASCII arrows/diagonals
				if (Math.Abs(nx) > Math.Abs(ny))
				{
					sb.Append(nx > 0 ? '>' : '<');
				}
				else if (Math.Abs(ny) > Math.Abs(nx))
				{
					sb.Append(ny > 0 ? '/' : '\\'); // use slashes to indicate up/down along Y
				}
				else
				{
					sb.Append('-');
				}
			}
		}

		Console.WriteLine(sb.ToString());
	}
}