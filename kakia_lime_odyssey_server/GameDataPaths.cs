namespace kakia_lime_odyssey_server;

/// <summary>
/// Centralized path constants for all game data files.
/// All paths are relative to the executable directory.
/// </summary>
public static class GameDataPaths
{
    public const string Root = "GameData";

    public static class Definitions
    {
        public const string Root = "GameData/Definitions";

        public static class Buffs
        {
            public const string BuffInfo = "GameData/Definitions/Buffs/BuffInfo.xml";
        }

        public static class Characters
        {
            public const string ExperienceTable = "GameData/Definitions/Characters/ExperienceTable.xml";
            public const string InheritTypes = "GameData/Definitions/Characters/InheritTypes.xml";
        }

        public static class Entities
        {
            public const string Monsters = "GameData/Definitions/Entities/Monsters.xml";
            public const string Models = "GameData/Definitions/Entities/Models.xml";
        }

        public static class Items
        {
            public const string Items = "GameData/Definitions/Items/Items.xml";
            public const string Crafting = "GameData/Definitions/Items/Crafting.xml";
            public const string Categories = "GameData/Definitions/Items/Categories.xml";
        }

        public static class Skills
        {
            public const string Skills = "GameData/Definitions/Skills/Skills.xml";
        }

        public const string QuestsFolder = "GameData/Definitions/Quests";
    }

    public static class World
    {
        public const string Root = "GameData/World";
        public const string MapsFolder = "GameData/World/Maps";
        public const string LocalAreaInfo = "GameData/World/LocalAreaInfo.xml";
        public const string MapTargetInfo = "GameData/World/MapTargetInfo.xml";
        public const string MobSpawns = "GameData/World/MobSpawns.xml";
        public const string NpcSpawns = "GameData/World/NpcSpawns.xml";
        public const string DropTables = "GameData/World/DropTables.xml";
    }
}
