using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Items : MonoBehaviour
{
   public enum WeaponType
{
    PawnBlade = 0,
    KnightLance = 1,
    BishopStaff = 2,
    RookHammer = 3,
    QueenRapier = 4,
    KingShield = 5,
    Sword=6
}

private static string[] items = new string[46]
{
    "Pawn's Dagger", "Knight's Lance", "Bishop's Staff", "Rook's Hammer", "Queen's Rapier","King's Sword", 
    "Checkmate Bow", "Castle Armor", "En Passant Boots", "Promotion Ring",
    "Opening Gambit Scroll", "Midgame Tonic", "Endgame Potion", "Check Scroll", "Mate Talisman",
    "Captured Piece Charm", "Pawn's Armor", "White Crown", "Black Crown", "Grandmaster's Tome",

    // Food and drink
    "Pawn Bread", "Knight's Stew", "Bishop's Wine", "Rook Biscuit", "Queen's Tea",
    "King's Feast", "Checkmate Cake", "Castling Cheese", "En Passant Eclair", "Promotion Pie",
    "Opening Omelette", "Midgame Muffin", "Endgame Espresso", "Check Chocolate", "Mate Macaron",
    "Captured Cookie", "Bishop's Honey", "Rook Roast", "Knight's Ale", "Queen's Sorbet",
    "King's Honeyed Bread", "Pawn Porridge", "Chessboard Candy", "Grandmaster's Coffee", "Royal Jelly",
    //something else
    "Twisted Sword"
};

private static string[] shortName = new string[46]
{
    "PawnDagger", "KnightLance", "BishopStaff", "RookHammer", "QueenRapier",
    "KingSword", "CheckBow", "CastleArmor", "EnPassBoots", "PromoRing",
    "GambitScrl", "MidTonic", "EndPotion", "CheckScrl", "MateTalis",
    "CaptCharm", "PawnArmor", "WhtCrown", "BlkCrown", "GMtome",

    "PawnBread", "KnightStew", "BishopWine", "RookBisc", "QueenTea",
    "KingFeast", "CheckCake", "CastleChz", "EnPassEcl", "PromoPie",
    "OpenOmlet", "MidMuffin", "EndEsp", "CheckChoc", "MateMac",
    "CaptCookie", "BishHoney", "RookRoast", "KnightAle", "QueenSorb",
    "KingBread", "PawnPorr", "BoardCandy", "GMCoffee", "RoyalJelly",
    "TwistSword"
};

private static string[] seriousName = new string[46]
{
    "PawnDagger", "KnightLance", "BishopStaff", "RookHammer", "QueenRapier",
    "KingSword", "CheckBow", "CastleArmor", "EnPassBoots", "PromotionRing",
    "GambitScroll", "MidTonic", "EndPotion", "CheckScroll", "MateTalisman",
    "CapturedCharm", "PawnArmor", "WhiteCrown", "BlackCrown", "GMtome",

    "PawnBread", "KnightStew", "BishopWine", "RookBiscuit", "QueenTea",
    "KingFeast", "CheckCake", "CastleCheese", "EnPassantEclair", "PromotionPie",
    "OpeningOmelette", "MidgameMuffin", "EndgameEspresso", "CheckChocolate", "MateMacaron",
    "CapturedCookie", "BishopHoney", "RookRoast", "KnightAle", "QueenSorbet",
    "KingsHoneyBread", "PawnPorridge", "BoardCandy", "GMCoffee", "RoyalJelly",
    "Sword"
};

private static string[] desc = new string[46]
{
    "* A small blade used by\n  pawns in close combat.",
    "* A long, curved lance\n  favored by knights.",
    "* A holy staff that channels\n  diagonal energy.",
    "* A crushing hammer that\n  smashes through ranks.",
    "* A swift rapier with\n  unmatched precision.",
    "* A long sword that's\n  too heavy to lift.",

    "* A bow that delivers\n  the final blow.",
    "* Heavy armor that\n  resembles a castle.",
    "* Boots granting sidestep\n  mastery like en passant.",
    "* A ring that promotes\n  its wearer's abilities.",
    "* A scroll describing bold\n  first moves.",
    "* A tonic that restores\n  mid-battle stamina.",
    "* A potion granting strength\n  in the final phase.",
    "* A scroll that forces\n  the foe into check.",
    "* A talisman ensuring\n  the final strike.",
    "* A charm holding the soul\n  of a fallen piece.",
    "* Armor patterned like\n  a chessboard.",
    "* A crown worn by the\n  white monarch.",
    "* A crown worn by the\n  black monarch.",
    "* The legendary tome of\n  chess wisdom.",

    "* A hearty bread served\n  to pawns on duty.",
    "* A rich stew favored by\n  mounted warriors.",
    "* A glass of fine wine\n  for clerics of the board.",
    "* A crumbly biscuit,\n  crunchy as a rook's walls.",
    "* A delicate tea with the\n  queen's personal blend.",
    "* A feast fit for a king.",
    "* A decadent cake celebrating\n  victory on the board.",
    "* Cheese aged in the\n  safety of a castle.",
    "* A delicate eclair named\n  after a swift capture.",
    "* A pie marking the rise\n  to power.",
    "* Eggs prepared with the\n  first move in mind.",
    "* A sweet muffin to\n  keep morale up.",
    "* A bitter espresso to\n  close out the game.",
    "* Dark chocolate that\n  corners the taste buds.",
    "* A small macaron for the\n  final victory.",
    "* A cookie taken from\n  a captured opponent.",
    "* Sweet honey gifted by\n  a bishop.",
    "* Slow-roasted meat with\n  rook's patience.",
    "* A strong ale that fuels\n  the charge.",
    "* A frozen dessert fit\n  for royalty.",
    "* Sweet bread glazed with\n  golden honey.",
    "* A warm porridge for\n  early moves.",
    "* Candy patterned like a\n  chessboard.",
    "* A cup of strong coffee\n  for deep planning.",
    "* Sweet, golden royal jelly.",
    "* A really strange blade.\n  Used for killing kings."
};
    //0 - food
    //1 - atk
    //2 - arm
    //3 - idk
private static int[] typesss = new int[46]
{
    0, 1, 2, 3, 4,
    5, 4, 3, 0, 2,
    2, 0, 0, 2, 2,
    1, 2, 5, 5, 2,

    // Food mostly type 0 (consumable)
    0, 0, 0, 0, 0,
    0, 0, 0, 0, 0,
    0, 0, 0, 0, 0,
    0, 0, 0, 0, 0,
    0, 0, 0, 0, 0,

    6
};
    private static int[] types = new int[46]
{
    1, 1, 1, 1, 1,
    1, 1, 2, 2, 2,
    2, 2, 0, 0, 0,
    0, 2, 2, 2, 2,

    // Food mostly type 0 (consumable)
    0, 0, 0, 0, 0,
    0, 0, 0, 0, 0,
    0, 0, 0, 0, 0,
    0, 0, 0, 0, 0,
    0, 0, 0, 0, 0,

    1
};


    private static int[] value = new int[46]
    {
        1, 20, 20, 25, 30, 99, 2, 3, 4, 5,
        10, 10, 24, 3, 5, 8, 1, 26, 18, 8,
        12, 8, 10, 24, 20, 30, 15, 15, 19, 25,
        18, 8, 5, 3, 3, 11, 11, 22, 15, 8,
        5, 12, 3, 10, 15, 99
    };
    private static Dictionary<int, int> weaponTypes = new Dictionary<int, int>
    {
        { 8, 1 },
        { 20, 2 },
        { 21, 3 },
        { 32, 2 },
        { 34, 4 },
        { 41, 5 }
    };

    public static string ItemName(int i)
    {
        if (i == -1)
        {
            return "None";
        }
        return items[i];
    }

    public static string ShortItemName(int i, bool isBoss)
    {
        if (isBoss)
        {
            return seriousName[i];
        }
        return shortName[i];
    }

    public static string ShortItemName(int i)
    {
        return ShortItemName(i, isBoss: false);
    }

    public static string ItemDescription(int i)
    {
        string text = "Unknown";
        if (ItemType(i) == 0)
        {
            text = ((i == 28) ? "Heals Some HP" : ((ItemValue(i) < 99) ? ("Heals " + ItemValue(i) + " HP") : "All HP"));
        }
        else if (ItemType(i) == 1)
        {
            text = ("ATK " + ItemValue(i));
        }
        else if (ItemType(i) == 2)
        {
            text = ("DEF " + ItemValue(i));
        }
        else if (ItemType(i) == 4)
        {
            text = "Heals " + ItemValue(i);
        }
        string text2 = desc[i];
        string text3 = "* \"";
        if(ItemName(i).Length>15)
            text3+=seriousName[i] + "\" - " + text + "\n" + text2;
        else
            text3+=ItemName(i) + "\" - " + text + "\n" + text2;
        /*
        if (ItemType(i) == 1)
        {
            text3 += "}* This weapon is a very\n  dangerous weapon.\n* One good hit is enough.";
        }*/
        return text3;
    }
    
    public static int ItemType(int i)
    {
        if (i == -1)
        {
            return -1;
        }
        return types[i];
    }

    public static int ItemValue(int i)
    {
        switch (i)
        {
            case -1:
                return 0;
        }
        return value[i];
    }

    public static string ItemUse(int i)
    {
        string text = "* You ";
        if (ItemType(i) == 0)
        {
            text += "ate the ";
            if (ItemName(i).Length < 15) 
                text+= ItemName(i) + "\n";
            else
                text += seriousName[i] + "\n";
            int hp = ItemValue(i);
            text += GetRecoveryString(hp);
        }
        else if (ItemType(i) == 1)
        {
            text += "equipped the ";
            if (ItemName(i).Length > 12)
                text += "\n  ";
            text += ItemName(i) + ".";
        }
        else if (ItemType(i) == 2)
        {
            text += "equipped the ";
            if (ItemName(i).Length > 12)
                text += "\n  ";
            text+= ItemName(i) + ".";
        }
        return text;
    }

    public static string ItemDrop(int i)
    {
        return "* The " + ItemName(i) + " was\n  thrown away.";
    }

    public static int NumOfItems()
    {
        return items.Length;
    }

    public static int GetHighestWeaponIndex()
    {
        int num;
        for (num = items.Length - 1; num >= 0; num--)
        {
            if (types[num] == 1)
            {
                return num;
            }
        }
        return num;
    }

    public static int GetHighestArmorIndex()
    {
        int num;
        for (num = items.Length - 1; num >= 0; num--)
        {
            if (types[num] == 2)
            {
                return num;
            }
        }
        return num;
    }

    public static string GetRecoveryString(int hp)
    {
        string array = "You ";
        string array2 = "Your";
        if (GameManager.instance.GetHP() + hp >= GameManager.instance.GetMaxHP())
        {
            return "* " + array2 + " HP was maxed out.";
        }
        return "* " + array + "recovered " + hp + " HP!";
    }

    public static int GetWeaponType(int i)
    {
        if (weaponTypes.ContainsKey(i))
        {
            return weaponTypes[i];
        }
        return 0;
    }

    public static string GetWeaponTypeName(int i)
    {
        string[] array = new string[6] { "PawnBlade","KnightLance","BishopStaff","RookHammer","QueenRapier","KingSword" };
        int weaponType = GetWeaponType(i);
        if (weaponType < array.Length)
        {
            return array[weaponType];
        }
        return "UNKNOWN (" + i + ")";
    }

    public static int GetItemElement(int i)
    {
        if (GetWeaponType(i) == 4 || GetWeaponType(i) == 1)
        {
            return 1;
        }
        return 0;
    }

    public static string GetBattleDescription(int i)
    {
        if (i < 0)
        {
            return "";
        }
        if (ItemType(i) == 0)
        {
            string text = ItemValue(i).ToString();
            switch (i)
            {
                case 5:
                    text = "all";
                    break;
                case 28:
                    text = "the";
                    break;
            }
            return "Heals " + text + " HP to one member";
        }
        if (ItemType(i) == 1)
        {
            return GetWeaponTypeName(i) + " Weapon (" + ItemValue(i) + " AT)";
        }
        if (ItemType(i) == 2)
        {
            return "Armor (" + ItemValue(i) + " DF)";
        }
        if (ItemType(i) == 4)
        {
            string text2 = ItemValue(i).ToString();
            return "Heals " + text2 + " HP to each member";
        }
        switch (i)
        {
            default:
                return "";
        }
    }

    public static List<int> GetItemsByType(int type, bool includeNone = false)
    {
        List<int> list = new List<int>();
        if (includeNone)
        {
            list.Add(-1);
        }
        for (int i = 0; i < NumOfItems(); i++)
        {
            if (types[i] == type || type == -1)
            {
                list.Add(i);
            }
        }
        return list;
    }

    public static List<string> GetItemNamesByType(int type, bool includeNone = false)
    {
        List<string> list = new List<string>();
        if (includeNone)
        {
            list.Add("None");
        }
        for (int i = 0; i < NumOfItems(); i++)
        {
            if (types[i] == type || type == -1)
            {
                list.Add(ItemName(i));
            }
        }
        return list;
    }
}
