namespace kakia_lime_odyssey_server;

/// <summary>
/// Centralized path constants for all game data files.
/// All paths are relative to the executable directory.
/// File names match original client XML names.
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
            public const string PcStatus = "GameData/Definitions/Characters/PcStatus.xml";
            public const string Inherit = "GameData/Definitions/Characters/inherit.xml";
            public const string PcJobClassInfo = "GameData/Definitions/Characters/PcJobClassInfo.xml";
            public const string PcCustomizingInfo = "GameData/Definitions/Characters/PcCustomizingInfo.xml";
        }

        public static class Entities
        {
            public const string Monsters = "GameData/Definitions/Entities/Monsters.xml";
            public const string ModelsInfo = "GameData/Definitions/Entities/ModelsInfo.xml";
            public const string ModelsTypeInfo = "GameData/Definitions/Entities/ModelsTypeInfo.xml";
        }

        public static class Items
        {
            public const string ItemInfo = "GameData/Definitions/Items/ItemInfo.xml";
            public const string ItemMakeInfo = "GameData/Definitions/Items/ItemMakeInfo.xml";
            public const string ItemSubjectInfo = "GameData/Definitions/Items/ItemSubjectInfo.xml";
            public const string EquipableJob = "GameData/Definitions/Items/equipableJob.xml";
            public const string EquipableRace = "GameData/Definitions/Items/equipableRace.xml";
        }

        public static class Skills
        {
            public const string SkillInfo = "GameData/Definitions/Skills/SkillInfo.xml";
        }

        public static class Quests
        {
            public const string Folder = "GameData/Definitions/Quests";
            public const string QuestList = "GameData/Definitions/Quests/questList.xml";
        }

        public static class Props
        {
            public const string ActivePropInfo = "GameData/Definitions/Props/ActivePropInfo.xml";
        }

        public static class Server
        {
            public const string ServerErrorInfo = "GameData/Definitions/Server/ServerErrorInfo.xml";
            public const string ForbiddenWords = "GameData/Definitions/Server/ForbiddenWords.xml";
            public const string StringTable = "GameData/Definitions/Server/StringTable.xml";
        }

        public static class Misc
        {
            public const string QuestIcons = "GameData/Definitions/Misc/QuestIcons.xml";
            public const string NameInfo = "GameData/Definitions/Misc/NameInfo.xml";
        }

        public const string QuestsFolder = "GameData/Definitions/Quests";
    }

    public static class World
    {
        public const string Root = "GameData/World";
        public const string MapsFolder = "GameData/World/Maps";
        public const string LocalAreaInfo = "GameData/World/LocalAreaInfo.xml";
        public const string MapTargetInfo = "GameData/World/MapTargetInfo.xml";
        public const string WorldmapTargetInfo = "GameData/World/WorldmapTargetInfo.xml";
        public const string MobSpawns = "GameData/World/MobSpawns.xml";
        public const string NpcSpawns = "GameData/World/NpcSpawns.xml";
        public const string DropTables = "GameData/World/DropTables.xml";
    }
}
