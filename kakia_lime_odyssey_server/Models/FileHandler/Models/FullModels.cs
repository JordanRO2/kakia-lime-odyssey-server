using kakia_lime_odyssey_server.Models.FileHandler.Terrain;
namespace kakia_lime_odyssey_server.Models.FileHandler.Models;

public class FullModels
{
	public Dictionary<QuadrantType, List<MdiModel>> Quadrants { get; set; }

	public FullModels(string mapName, string folder)
	{
		var files = Directory.GetFiles(folder, $"{mapName}.gmdi*");
		Quadrants = new Dictionary<QuadrantType, List<MdiModel>>();
		foreach (var file in files)
		{
			MdiParser mdiParser = new MdiParser(file);
			var modelData = mdiParser.Parse();


			var quadrant = (QuadrantType)int.Parse(file.Last().ToString());
			var models = modelData.ContainsKey("models") ? modelData["models"] as List<MdiModel> : [];
			if (models is not null)
				Quadrants.Add(quadrant, models);
		}
	}
}
