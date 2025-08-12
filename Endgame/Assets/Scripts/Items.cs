using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Items : MonoBehaviour
{
    public enum WeaponType
    {
        Slash = 0,
        IceRing = 1,
        Quad = 2,
        Bash = 3,
        IceSlash = 4,
        Katana = 5
    }

    private static string[] items = new string[46]
    {
        "Test Food", "Test Knife", "Test Chest", "Pencil", "Bandage", "Butterscotch Pie", "Big Pencil", "Bandage", "Snow Ring", "Wristwatch",
        "Monster Candy", "Spider Donut", "Spider Cider", "Toy Knife", "Faded Ribbon", "Heavy Branch", "Egg", "Chocolate Candy", "Quiet Shroom", "Hard Hat",
        "Clean Pan", "Cracked Bat", "Skip Sandwich", "Hamburger", "Punch Card", "Stick", "Antique Knife", "Real Knife", "Snail Pie", "Banana",
        "Boiled Egg", "Aluminum Bat", "Tough Glove", "Manly Bandanna", "Permacicle", "Bisicle", "Unisicle", "Cinnamon Bun", "Nice Cream", "Carrot",
        "Blood Bandana", "Bluster Blade", "Papyrus Charm", "Old Tutu", "Spaghetti", "WILD REVERSE CARD"
    };

    private static string[] shortName = new string[46]
    {
        "Test Food", "TestKnife", "TestChest", "Pencil", "Bandage", "ButtsPie", "BigPencil", "Bandage", "Snow Ring", "Wrstwatch",
        "MnstrCndy", "SpidrDont", "SpidrCidr", "Toy Knife", "Ribbon", "HevyBrnch", "Egg", "ChocCandy", "Shhhhroom", "Hard Hat",
        "Clean Pan", "CrackdBat", "SkpSndwch", "Hamburger", "PunchCard", "Stick", "A. Knife", "RealKnife", "Snail Pie", "Banana",
        "BoiledEgg", "Al Bat", "TuffGlove", "Mandanna", "Prmacicle", "Bisicle", "Unisicle", "CinnaBun", "NiceCream", "Carrot",
        "Bloodana", "BustBlade", "PapyCharm", "Old Tutu", "Spaghetti", "REVRSCARD"
    };

    private static string[] seriousName = new string[46]
    {
        "Test Food", "TestKnife", "TestChest", "Pencil", "Bandage", "Pie", "BigPencil", "Bandage", "Snow Ring", "Watch",
        "MnstrCndy", "SpdrDonut", "SpdrCider", "Toy Knife", "Ribbon", "Branch", "Egg", "Chocolate", "QuiShroom", "Hard Hat",
        "Clean Pan", "CrackdBat", "SkpSndwch", "Hamburger", "PunchCard", "Stick", "AntqKnife", "RealKnife", "Snail Pie", "Banana",
        "BoiledEgg", "AlmnumBat", "Glove", "Bandanna", "Icicle", "Bisicle", "Unisicle", "C. Bun", "NiceCream", "Carrot",
        "BBandana", "Katana", "Charm", "Tutu", "Spaghett", "SkipCard"
    };

    private static string[] desc = new string[46]
    {
        "* Tastes odd, but good for\n  testing.", "* It's super sharp!", "* There's a scribbled \"R\"\n  in the middle.", "* Mightier than a sword?\n* Maybe equal at best.", "* It has cartoon characters on\n  it.", "* Butterscotch-cinnamon\n  pie, one slice.", "* The eraser end is completely\n  bitten off.", "* It has cartoon characters on\n  it.", "* For some reason, it feels\n  really cold in your hands.", "* Maybe an expensive antique.\n* Stuck before half past noon.",
        "* Has a distinct,\n^10  non-licorice flavor.", "* A donut made with Spider\n  Cider in the batter.", "* Made with whole spiders,\n  not just the juice.", "* Made of plastic.\n* A rarity nowadays.", "* If you're cuter,^10 monsters\n  won't hit you as hard.", "* A big branch straight\n  off the Snowdin trees.", "* Not too important, not\n  too unimportant.", "* A rich,^05 dark chocolate treat.", "* A reformed mushroom that is\n  neither ramblin' nor evil.", "* Construction cap intended for\n  construction sites.",
        "* A powerful,^05 non-burnt pan.\n* Has no passive effects.", "* A light,^05 wooden bat with\n  a noticable crack on it.", "* A sea-based sandwich that\n  increases SPEED in battle.}* Additionally,^05 eating it out of\n  battle will increase your\n  base speed for the room.", "* A burger,^05 cooked to perfection.\n* Likely made of magic.", "* Use to make Susie stronger\n  in one battle.}* Use outside of battle to\n  look at the card.", "* Its bark is worse than\n  its bite.", "* A well-worn blade belonging\n  to Susie's lost friend.", "* Here we are!", "* An acquired taste.", "* Potassium.",
        "* Finally,^05 an egg that you\n  can eat.", "* A powerful bat that's also\n  lightweight.", "* A worn pink leather glove.\n* For five-fingered folk.", "* It has seen some wear.\n* It has abs drawn on it.}* The lower the wearer's HP goes,^05\n  the higher damage they'll deal.}* However,^05 they will deal less\n  damage when closer to full\n  health than normal.", "* A magical icicle that doesn't\n  melt or smoke away.", "* It's a two-pronged popsicle,^05\n  so you can eat it twice.", "* It's a SINGLE-pronged popsicle.\n* Wait,^05 that's just normal...", "* A cinnamon roll in the shape\n  of a bunny.", "* Instead of a joke,^05 the\n  wrapper says something nice.", "* Orange plant object.\n* Presumably worn by a snowman.",
        "* A bright red bandana that gives\n  off the essence of blood.}* Wearing this bandana will\n  increase the range bullets\n  increase tension.", "* A katana infused with WIND\n  magic.", "* A shiny pendant bearing\n  Papyrus's likeness.}* If worn,^05 the wearer won't take\n  any damage when hit for the\n  first time in battle.}* This effect does not work in\n  overworld bullet segments.", "* Its age is what makes it\n  so protective.", "* A large pasta pot,^05 enough\n  for three servings.", "* A strange,^05 non-standard UNO\n  card.}* In UNO,^05 it reverses player\n  order and changes color.^05\n* Skips enemy turn in battle."
    };

    private static int[] types = new int[46]
    {
        0, 1, 2, 1, 2, 0, 1, 0, 1, 2,
        0, 0, 0, 1, 2, 1, 3, 0, 0, 2,
        1, 1, 0, 0, 3, 1, 1, 1, 0, 0,
        0, 1, 1, 2, 1, 0, 0, 0, 0, 0,
        2, 1, 2, 2, 4, 3
    };

    private static int[] value = new int[46]
    {
        999, 200, 20, 1, 1, 999, 2, 10, 1, 5,
        10, 12, 24, 3, 5, 8, 0, 26, 18, 8,
        12, 8, 10, 24, 0, 1, 15, 15, 999, 25,
        18, 8, 5, 3, 3, 11, 11, 22, 15, 8,
        5, 12, 3, 10, 15, 0
    };

    private static int[] sellPrice = new int[46]
    {
        -1, -1, -1, 30, 30, 100, 1, 30, -1, 200,
        15, 25, 40, 50, 50, 2, -1, 66, 5, 60,
        -1, 60, 27, 50, 72, -1, -1, -1, -1, 69,
        6, 80, 75, 75, 20, 40, 20, 30, 60, 8,
        150, 150, -1, 100, 150, 250
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

    private static Dictionary<int, int> magicValue = new Dictionary<int, int>
    {
        { 8, 5 },
        { 34, 4 },
        { 42, 2 }
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
            text = ((GetItemMagic(i) <= 0) ? ("Weapon AT " + ItemValue(i)) : ("Wpn AT " + ItemValue(i) + " MG " + GetItemMagic(i)));
        }
        else if (ItemType(i) == 2)
        {
            text = ((GetItemMagic(i) <= 0) ? ("Armor DF " + ItemValue(i)) : ("Amr DF " + ItemValue(i) + " MG " + GetItemMagic(i)));
        }
        else if (ItemType(i) == 4)
        {
            text = "Heals " + ItemValue(i) + " HP Each";
        }
        else if (i == 24 || i == 45)
        {
            text = "Battle Item";
        }
        string text2 = desc[i];
        string text3 = "* \"" + ItemName(i) + "\" - " + text + "\n" + text2;
        if (ItemType(i) == 1)
        {
            if (GetWeaponType(i) == 0)
            {
                text3 += "}* This weapon is a SLASH\n  type weapon.\n* One bar, standard damage.";
            }
            else if (GetWeaponType(i) == 1)
            {
                text3 += "}* This ICERING allows Noelle to\n  cast ICE spells when equipped.";
            }
            else if (GetWeaponType(i) == 2)
            {
                text3 += "}* This QUAD-type weapon uses\n  four bars instead of one.\n* More crits means more damage.";
                if (i == 32)
                {
                    text3 += "}* However,^05 this specific weapon\n  has less incremental crit\n  damage than other QUAD-types.";
                }
            }
            else if (GetWeaponType(i) == 3)
            {
                text3 += "}* This is a BASH type weapon.\n* The bar progressively gets\n  faster with time.";
            }
            else if (GetWeaponType(i) == 4)
            {
                text3 += "}* This weapon is a ICESLASH\n  type weapon.\n* One bar, standard ICE damage.}* If equipped to Noelle,^05 she will\n  be able to cast ICE spells.}* However,^05 its effect on HEAL\n  PRAYER will be weaker than\n  other MAGIC weapons.";
            }
            else if (GetWeaponType(i) == 5)
            {
                text3 += "}* This blade has two bars.^05\n* One aims vertically,^05 the other\n  horizontally.}* Lining them both up perfectly\n  will damage all enemies on\n  screen.}* However,^05 hitting off-target is\n  penalized with less damage\n  than SLASH weapons.";
            }
        }
        if (i == 16)
        {
            text3 = "* \"Egg\" - Not too important, not\n  too unimportant.";
        }
        if (i == 45)
        {
            text3 = "* \"WILD REVERSE CARD\" - Card\n" + text2;
        }
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

    public static int ItemValue(int i, int partyMember = 0)
    {
        switch (i)
        {
            case -1:
                return 0;
            case 23:
                if (partyMember == 2)
                {
                    return value[i] / 2;
                }
                break;
        }
        return value[i];
    }

    public static string ItemUse(int i, int from, int to, bool serious)
    {
        string[] array = new string[4] { "You ", "Susie ", "Noelle ", "Paula " };
        _ = new string[4] { "Your", "Susie's", "Noelle's", "Paula's" };
        string text = "* " + array[from];
        string text2 = ItemName(i);
        /*
        if ((to == 1 || to == 2) && (int)Object.FindObjectOfType<GameManager>().GetFlag(172) == 2)
        {
            serious = true;
        }
        if (to == 2 && (int)Object.FindObjectOfType<GameManager>().GetFlag(172) == 1)
        {
            serious = true;
        }
        */
        if (from != to)
        {
            if (from == 2 && i == 5)
            {
                text2 = "Pie";
            }
            text = text + "gave the " + text2 + "\n  to " + ((to == 0) ? "you " : array[to]);
        }
        if (ItemType(i) == 0)
        {
            if (i == 38)
            {
                text = (new string[7] { "* You're just great!\n", "* You look nice today!\n", "* Are those claws natural?\n", "* You're super spiffy!\n", "* Have a wonderful day!\n", "* Is this as sweet as you?\n", "* Love yourself! I love you!\n" })[Random.Range(0, 7)];
            }
            else if (from == to)
            {
                switch (i)
                {
                    case 7:
                        text += "re-applied the bandage.\n";
                        break;
                    case 12:
                        text = text + "drank the " + ItemName(i) + ".\n";
                        break;
                    case 35:
                        text += "ate one half of the\n  Bisicle.\n";
                        break;
                    default:
                        text = text + "ate the " + ItemName(i) + ".\n";
                        break;
                }
                if (from == 0 && i == 5)
                {
                    text += "* It tastes like home.\n";
                }
            }
            else
            {
                string text3 = ((to == 0) ? "you" : "she");
                switch (i)
                {
                    case 7:
                        text += "and reapplied it.\n";
                        break;
                    case 12:
                        text = text + "and " + text3 + " drank it.\n";
                        break;
                    case 35:
                        text = "* " + array[to] + "ate one half of the\n  Bisicle.\n";
                        break;
                    default:
                        text = text + "and " + text3 + " ate it.\n";
                        break;
                }
            }
            int hp = ItemValue(i, to);
            if (i == 28 && to != 1)
            {
                hp = Util.GameManager().GetMaxHP(to) - Util.GameManager().GetHP(to) - 1;
            }
            if (i == 39 && to == 2)
            {
                hp = 999;
            }
            text += GetRecoveryString(to, hp);
        }
        else if (ItemType(i) == 1)
        {
            if (from == to)
            {
                text = ((ItemName(i).Length <= 13) ? (text + "equipped the " + ItemName(i) + ".") : (text + "equipped the\n  " + ItemName(i) + "."));
            }
            else
            {
                string text4 = ((to == 0) ? "you" : "she");
                text = text + "and " + text4 + " equipped it.\n";
            }
            if (i == 27)
            {
                text = "* How convenient.";
            }
            if (to == 1)
            {
                switch (i)
                {
                    case 20:
                        text = "su_depressed`snd_txtsus`* ...I'm not taking\n  this.";
                        break;
                    case 27:
                        text = (serious ? "su_pissed`snd_txtsus`* Stop pointing that\n  thing at me!!!" : "su_side_sweat`snd_txtsus`* (Can they stop pointing\n  that at me...?)");
                        break;
                    default:
                        break;
                }
            }
            if (to == 2 && i == 41)
            {
                text = (serious ? ("* Noelle declined to equip the\n  " + ItemName(i) + ".") : "no_happy`snd_txtnoe`* S-sorry Kris,^05 but that's\n  too heavy for me\n  to use...");
            }
        }
        else if (ItemType(i) == 2)
        {
            if (from == to)
            {
                text = ((ItemName(i).Length <= 13) ? (text + "equipped the " + ItemName(i) + ".") : (text + "equipped the\n  " + ItemName(i) + "."));
            }
            else
            {
                string text5 = ((to == 0) ? "you" : "she");
                text = text + "and " + text5 + " equipped it.\n";
            }
            if (to == 1 && (i == 14 || i == 43))
            {
                if (serious)
                {
                    text = "* Susie declined to equip the\n  " + ItemName(i) + ".";
                }
                else
                {
                    switch (i)
                    {
                        case 43:
                            text = "su_annoyed`snd_txtsus`* Over my dead body.";
                            break;
                        case 14:
                            text = "su_annoyed`snd_txtsus`* No way.";
                            break;
                    }
                }
            }
        }
        else if (ItemType(i) == 4)
        {
            text = "* Everyone ate the " + ItemName(i) + "\n  and recovered " + ItemValue(i) + " HP each!";
        }
        else
        {
            switch (i)
            {
                case 24:
                    text = ((from != 1 && to != 1) ? "* Susie took the punch card.\n" : ((from != 1 || to == 1) ? "" : "* Susie did not give the card.\n"));
                    text += "* OOOORAAAAA!!!\n* Susie rips up the punch card!}* Susie's AT increased by 10!";
                    break;
                case 16:
                    text = ((to == 0) ? ((from == 0) ? "* You used the Egg." : ("* " + array[from] + "held the Egg.\n^10* She gave it back to you.")) : ("* You gave the egg to " + ((to == 1) ? "Susie" : "Noelle") + ".^10\n* She gave it back."));
                    break;
                default:
                    text = text + "used the " + ItemName(i) + ".";
                    if (ItemType(i) == 3)
                    {
                        text += "\n* Something occurred.";
                    }
                    break;
            }
        }
        if (SceneManager.GetActiveScene().buildIndex == 123)
        {
            if (text.StartsWith("su_"))
            {
                text = text.Replace("su_", "suhd_");
            }
            if (text.StartsWith("no_"))
            {
                text = text.Replace("no_", "nohd_");
            }
        }
        return text;
    }

    public static string ItemDrop(int i)
    {
        if (i == 16)
        {
            return "* What Egg?";
        }
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

    public static string GetRecoveryString(int partyMember, int hp)
    {
        string[] array = new string[4] { "You ", "Susie ", "Noelle ", "Paula " };
        string[] array2 = new string[4] { "Your", "Susie's", "Noelle's", "Paula's" };
        if (Object.FindObjectOfType<GameManager>().GetHP(partyMember) + hp >= Object.FindObjectOfType<GameManager>().GetMaxHP(partyMember))
        {
            if (partyMember > 2 && Object.FindObjectOfType<GameManager>().GetHP(partyMember) + hp > Object.FindObjectOfType<GameManager>().GetMaxHP(partyMember))
            {
                return "* " + array2[partyMember] + " HP overflowed to you!";
            }
            return "* " + array2[partyMember] + " HP was maxed out.";
        }
        return "* " + array[partyMember] + "recovered " + hp + " HP!";
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
        string[] array = new string[6] { "SLASH", "ICERING", "QUAD", "BASH", "ICESLASH", "KATANA" };
        int weaponType = GetWeaponType(i);
        if (weaponType < array.Length)
        {
            return array[weaponType];
        }
        return "UNKNOWN (" + i + ")";
    }

    public static int GetItemMagic(int i)
    {
        if (magicValue.ContainsKey(i))
        {
            return magicValue[i];
        }
        return 0;
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
            case 24:
                return "Increases Susie's AT by 10";
            case 45:
                return "Skips enemy turn";
            default:
                return "";
        }
    }

    public static int GetSellPrice(int i)
    {
        if (i >= sellPrice.Length || i < 0)
        {
            return -1;
        }
        return sellPrice[i];
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
