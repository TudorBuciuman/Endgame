using System.Collections.Generic;

public class MapInfo
{
    private static Dictionary<int, string> validSavePoints = new Dictionary<int, string>
    {
        { 6, "Ruins - Pawn's Square" },
        { 7, "Board - Knight's Stable" },
        { 8, "Board - Bishop's Chapel" },
        { 9, "Board - Rook's Corridor" },
        { 11, "Board - Queen's Mansion" },
        { 12, "City - White Kingdom" },
        { 13, "Castle - Throne Room" },
        { 14, "City - White Kingdom" },

        { 37, "Ruins - Home" },
        { 49, "Snowdin - Box Road" },
        { 51, "Twoson Caves" },
        { 56, "Happy Happy Village" },
        { 63, "???" },
        { 66, "LOSTCORE - End" },
        { 74, "Snowdin - Box Road" },
        { 86, "Snowdin - Spaghetti" },
        { 53, "Peaceful Rest Valley" },
        { 70, "Lilliput Steps Cave" },
        { 91, "Snowdin - Dogi Checkpoint" },
        { 88, "Snowdin - Bunny House" },
        { 95, "Snowdin - Dog House" },
        { 99, "Snowdin - Cave Entrance" },
        { 107, "Snowdin - Snow Pile" },
        { 108, "Snowdin - Under Bridge" },
        { 111, "Snowdin - Town" },
        { 121, "Waterfall - Checkpoint" }
    };

    private static Dictionary<int, string> savePlatforms = new Dictionary<int, string>
    {
        { 4, "waterfall" },
        { 5, "waterfall" },
        { 9, "ruins" },
        { 15, "ruins" },
        { 21, "ruins" },
        { 37, "ruins" },
        { 49, "snowdin" },
        { 51, "eb" },
        { 56, "eb" },
        { 66, "lostcore" },
        { 74, "snowdin_gguf" },
        { 86, "snowdin_gguf" },
        { 53, "eb" },
        { 70, "eb" },
        { 91, "snowdin_gguf" },
        { 88, "snowdin_gguf" },
        { 95, "snowdin_gguf" },
        { 99, "snowdin_gguf" },
        { 108, "snowdin_gguf" },
        { 111, "snowdin" },
        { 121, "waterfall" }
    };

    public static string GetMapName(int bIndex)
    {
        if (validSavePoints.ContainsKey(bIndex))
        {
            return validSavePoints[bIndex];
        }
        return "";
    }

    public static string GetMapSavePlatform(int bIndex)
    {
        if (savePlatforms.ContainsKey(bIndex))
        {
            return savePlatforms[bIndex];
        }
        return "";
    }

    public static bool IsValidMapSpawn(int map)
    {
        return validSavePoints.ContainsKey(map);
    }

    public static List<string> GetMapNames()
    {
        return new List<string>(validSavePoints.Values);
    }

    public static List<int> GetMapIDs()
    {
        return new List<int>(validSavePoints.Keys);
    }
}
