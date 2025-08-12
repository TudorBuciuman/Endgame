using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveFile
{
    public string name;

    public int exp;

    public List<int> items;

    public int[] weapon;

    public int[] armor;

    public int zone;

    public int gold;

    public object[] flags;

    public object[] persFlags;

    public string zoneName;

    public int deaths;

    public int scene; //1-fighting
                      //2-rising
                      //3-falling
}
