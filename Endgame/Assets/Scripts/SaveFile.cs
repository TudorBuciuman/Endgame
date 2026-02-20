using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SAVEFile
{
    public string name;

    public int exp;

    public List<int> items;

    public int weapon;

    public int armor;

    public int zone;

    public int gold;

    public object[] flags;

    public object[] persFlags;

    public string zoneName;

    public int deaths;

    public int playTime;

    public int scene; //1-fighting
                      //2-rising
                      //3-falling
    public Vector2 pos;
    public void UpdateCharacterInfo(string name, int exp, List<int> items, int weapon, int armor, int playTime, int zone, int gold, int scene,Vector2 pos, string zoneName, object[] flags)
    {
        this.name = name;
        this.exp = exp;
        this.items = new List<int>(items);
        this.weapon = weapon;
        this.armor = armor;
        this.playTime = playTime;
        this.zone = zone;
        this.gold = gold;
        this.scene = scene;
        this.pos = pos;
        this.zoneName = zoneName;
        this.flags = (object[])flags.Clone();
    }

    public void UpdateDeathCount(int deaths)
    {
        this.deaths = deaths;
    }

    public void UpdatePersistentFlags(object[] persFlags)
    {
        this.persFlags = persFlags;
    }
}
