using System.Xml.Serialization;
using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_server.Entities.Npcs;

namespace kakia_lime_odyssey_server.Models;

/// <summary>
/// Loads world configuration data from XML files (spawns, loot tables)
/// </summary>
public static class WorldDataLoader
{
    private static List<Npc>? _npcCache;
    private static List<MapMob>? _mobCache;
    private static Dictionary<int, List<LootableItem>>? _lootCache;

    public static List<Npc> LoadNpcSpawns()
    {
        if (_npcCache != null) return _npcCache;

        if (!File.Exists(GameDataPaths.World.NpcSpawns))
        {
            _npcCache = new List<Npc>();
            return _npcCache;
        }

        XmlSerializer serializer = new XmlSerializer(typeof(NpcSpawnsXml));
        using FileStream fs = new FileStream(GameDataPaths.World.NpcSpawns, FileMode.Open);
        var data = (NpcSpawnsXml)serializer.Deserialize(fs)!;
        _npcCache = data.Npcs.Select(n => n.ToNpc()).ToList();
        return _npcCache;
    }

    public static List<MapMob> LoadMobSpawns()
    {
        if (_mobCache != null) return _mobCache;

        if (!File.Exists(GameDataPaths.World.MobSpawns))
        {
            _mobCache = new List<MapMob>();
            return _mobCache;
        }

        XmlSerializer serializer = new XmlSerializer(typeof(MobSpawnsXml));
        using FileStream fs = new FileStream(GameDataPaths.World.MobSpawns, FileMode.Open);
        var data = (MobSpawnsXml)serializer.Deserialize(fs)!;
        _mobCache = data.Mobs.Select(m => m.ToMapMob()).ToList();
        return _mobCache;
    }

    public static Dictionary<int, List<LootableItem>> LoadDropTables()
    {
        if (_lootCache != null) return _lootCache;

        if (!File.Exists(GameDataPaths.World.DropTables))
        {
            _lootCache = new Dictionary<int, List<LootableItem>>();
            return _lootCache;
        }

        XmlSerializer serializer = new XmlSerializer(typeof(DropTablesXml));
        using FileStream fs = new FileStream(GameDataPaths.World.DropTables, FileMode.Open);
        var data = (DropTablesXml)serializer.Deserialize(fs)!;

        _lootCache = new Dictionary<int, List<LootableItem>>();
        foreach (var table in data.Tables)
        {
            _lootCache[table.Id] = table.Drops.Select(d => new LootableItem
            {
                Id = d.ItemId,
                DropRate = d.Rate
            }).ToList();
        }
        return _lootCache;
    }
}

#region XML Models

[XmlRoot("NpcSpawns")]
public class NpcSpawnsXml
{
    [XmlElement("Npc")]
    public List<NpcSpawnXml> Npcs { get; set; } = new();
}

public class NpcSpawnXml
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("zoneId")]
    public int ZoneId { get; set; }

    [XmlAttribute("modelTypeId")]
    public int ModelTypeId { get; set; }

    [XmlAttribute("typeId")]
    public int TypeId { get; set; }

    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;

    [XmlElement("Pos")]
    public XmlVector3 Pos { get; set; } = new();

    [XmlElement("Dir")]
    public XmlVector3 Dir { get; set; } = new();

    [XmlElement("Status")]
    public XmlNpcStatus Status { get; set; } = new();

    public Npc ToNpc()
    {
        return new Npc
        {
            Id = Id,
            ZoneId = (uint)ZoneId,
            Pos = new FPOS { x = Pos.X, y = Pos.Y, z = Pos.Z },
            Dir = new FPOS { x = Dir.X, y = Dir.Y, z = Dir.Z },
            Status = new COMMON_STATUS
            {
                lv = (byte)Status.Level,
                hp = (uint)Status.Hp,
                mhp = (uint)Status.MaxHp,
                mp = (uint)Status.Mp,
                mmp = (uint)Status.MaxMp
            },
            Appearance = new APPEARANCE_VILLAGER
            {
                appearance = new APPEARANCE_NPC
                {
                    name = System.Text.Encoding.UTF8.GetBytes(Name.PadRight(31, '\0')),
                    modelTypeID = (uint)ModelTypeId,
                    typeID = (uint)TypeId,
                    scale = 1.0f,
                    transparent = 1.0f
                },
                specialistType = 0
            }
        };
    }
}

public class XmlNpcStatus
{
    [XmlAttribute("lv")]
    public int Level { get; set; } = 1;

    [XmlAttribute("hp")]
    public int Hp { get; set; } = 300;

    [XmlAttribute("mhp")]
    public int MaxHp { get; set; } = 300;

    [XmlAttribute("mp")]
    public int Mp { get; set; } = 448;

    [XmlAttribute("mmp")]
    public int MaxMp { get; set; } = 448;
}

[XmlRoot("MobSpawns")]
public class MobSpawnsXml
{
    [XmlElement("Mob")]
    public List<MobSpawnXml> Mobs { get; set; } = new();
}

public class MobSpawnXml
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("zoneId")]
    public int ZoneId { get; set; }

    [XmlAttribute("modelTypeId")]
    public int ModelTypeId { get; set; }

    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;

    [XmlAttribute("lootTableId")]
    public int LootTableId { get; set; }

    [XmlElement("Pos")]
    public XmlVector3 Pos { get; set; } = new();

    public MapMob ToMapMob()
    {
        return new MapMob
        {
            Id = Id,
            ZoneId = ZoneId,
            ModelTypeId = ModelTypeId,
            Name = Name,
            LootTableId = LootTableId,
            Pos = new FPOS { x = Pos.X, y = Pos.Y, z = Pos.Z }
        };
    }
}

public class XmlVector3
{
    [XmlAttribute("x")]
    public float X { get; set; }

    [XmlAttribute("y")]
    public float Y { get; set; }

    [XmlAttribute("z")]
    public float Z { get; set; }
}

[XmlRoot("DropTables")]
public class DropTablesXml
{
    [XmlElement("DropTable")]
    public List<DropTableXml> Tables { get; set; } = new();
}

public class DropTableXml
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlElement("Drop")]
    public List<DropEntryXml> Drops { get; set; } = new();
}

public class DropEntryXml
{
    [XmlAttribute("itemId")]
    public int ItemId { get; set; }

    [XmlAttribute("rate")]
    public double Rate { get; set; }
}

#endregion
