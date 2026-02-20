using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator
{
    private static Dictionary<int, Type[]> enemies = new Dictionary<int, Type[]>
    {
        {
            1,
            new Type[1] { typeof(BladeKnight) }
        },
        /*
        {
            1,
            new Type[1] { typeof(WhiteKing) }
        },*/

    };

    private static Dictionary<int, float[]> xValues = new Dictionary<int, float[]>
    {
        {
            1,
            new float[1]
        },
    };

    private static Dictionary<int, object[]> music = new Dictionary<int, object[]>
    {
        
        {
            1,
            //new object[2] { "music/mus_lostcore_battle", 1 }
            new object[2] { "", 1 }
        },
    };

    private static Dictionary<int, string> approach = new Dictionary<int, string>
    {
        { 1, "* BLADEKNIGHT appears." },
    };

    private static Dictionary<int, object[]> bg = new Dictionary<int, object[]>
    {
        
        {
            1,
            new object[5]
            {
                1,
                0.1f,
                60,
                (Color)new Color32(123, 123, 123, byte.MaxValue),
                false
            }
        },
    };

    private static Dictionary<int, object[]> fallbackBG = new Dictionary<int, object[]>
    {
        {
            1,
            new object[5]
            {
                1,
                0.1f,
                60f,
                (Color)new Color32(239, 239, 23, byte.MaxValue),
                false
            }
        },
        {
            2,
            new object[5]
            {
                1,
                0.8f,
                60f,
                (Color)new Color32(104, 120, 4, byte.MaxValue),
                false
            }
        },
        {
            3,
            new object[5]
            {
                2,
                5f,
                5f,
                (Color)new Color32(192, 56, 152, byte.MaxValue),
                false
            }
        },
        {
            5,
            new object[5]
            {
                1,
                1f,
                60f,
                (Color)new Color32(128, 24, 168, byte.MaxValue),
                false
            }
        },
        {
            6,
            new object[5]
            {
                1,
                0.3f,
                60f,
                (Color)new Color32(0, 0, 196, byte.MaxValue),
                true
            }
        },
        {
            7,
            new object[5]
            {
                2,
                4f,
                60f,
                (Color)new Color32(239, 239, 23, byte.MaxValue),
                false
            }
        },
        {
            8,
            new object[5]
            {
                1,
                1f,
                30f,
                (Color)new Color32(176, 176, 120, byte.MaxValue),
                false
            }
        },
        {
            9,
            new object[5]
            {
                2,
                2f,
                100f,
                (Color)new Color32(176, 176, 120, byte.MaxValue),
                false
            }
        },
        {
            10,
            new object[5]
            {
                2,
                1f,
                80f,
                (Color)new Color32(168, 24, 88, byte.MaxValue),
                true
            }
        },
        {
            11,
            new object[5]
            {
                1,
                1f,
                60f,
                Color.red,
                true
            }
        },
        {
            12,
            new object[5]
            {
                1,
                0.1f,
                60f,
                Color.blue,
                true
            }
        }
    };



    public static EnemyBase[] GetEnemies(int battleId)
    {
        if (enemies.ContainsKey(battleId))
        {
            int num = enemies[battleId].Length;
            if (num > 3)
            {
                num = 3;
            }
            if (num < xValues[battleId].Length)
            {
                num = xValues[battleId].Length;
            }
            EnemyBase[] array = new EnemyBase[num];
            for (int i = 0; i < num; i++)
            {
                array[i] = new GameObject("Enemy" + (i + 1), enemies[battleId][i]).GetComponent<EnemyBase>();
                array[i].transform.position = new Vector2(xValues[battleId][i], 0f);
            }
            return array;
        }
        return null;
    }

    public static object[] GetMusic(int battleId)
    {
        if (music.ContainsKey(battleId))
        {
            if (music[battleId].Length < 1)
            {
                return new object[2]
                {
                    music[battleId][0],
                    1
                };
            }
            return music[battleId];
        }
        return new object[2] { "music/mus_battle", 1 };
    }

    public static string GetApproachText(int battleId)
    {
        Dictionary<int, string> serializedClass = Util.PackManager().GetSerializedClass<Dictionary<int, string>>("EnemyGenerator");
        if (approach.ContainsKey(battleId))
        {
            if (serializedClass != null && serializedClass.ContainsKey(battleId))
            {
                return serializedClass[battleId];
            }
            return approach[battleId];
        }
        return Util.MiscStrings().GetString("default_enemy_approach", 0);
    }

    public static object[] GetBattleBG(int battleId)
    {
        object[] array = new object[5]
        {
            0,
            0,
            0,
            new Color(0.1333f, 0.694f, 0.298f),
            false
        };
        if (bg.ContainsKey(battleId))
        {
            array = bg[battleId];
        }
        int num = (int)array[0];
        int key = (int)float.Parse(array[1].ToString());
        return array;
    }

    public static int GetEncounterCount()
    {
        return enemies.Count;
    }

    public static string GetEncounterName(int battleId)
    {
        if (enemies[battleId] == null)
        {
            return "EMPTY DO NOT USE";
        }
        List<string> list = new List<string>();
        Type[] array = enemies[battleId];
        foreach (Type type in array)
        {
            list.Add(type.ToString());
        }
        return string.Join(", ", list.ToArray());
    }
}
