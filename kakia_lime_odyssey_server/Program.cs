using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_server.Models;
using kakia_lime_odyssey_server.Models.FileHandler;
using kakia_lime_odyssey_server.Network;
using Newtonsoft.Json;
using System.Text;


Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
Logger.SetLogLevel(LogLevel.Debug);

/* 
 * Used for generating navmeshes (as well as testing it by creating navmesh images and height/lightmap images)
 * 

var mapNames = Directory.GetFiles(GameDataPaths.World.MapsFolder, "*.trn*")
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

// Handle both interactive console and background/service mode
if (Console.IsInputRedirected || !Environment.UserInteractive)
{
	// Running in background/service mode - wait indefinitely
	await Task.Delay(Timeout.Infinite, ct.Token);
}
else
{
	// Running interactively - wait for Q key
	while (Console.ReadKey(true).KeyChar.ToString().ToLower() != "q")
	{
		await Task.Delay(100);
	}
}
server.Stop();
ct.Cancel();