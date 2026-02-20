using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public abstract class TranslatableBehaviour : MonoBehaviour
{
    public string stringSubFolder = "";

    public Dictionary<string, string[]> strings;

    private Dictionary<string, string[]> GetStrings(Dictionary<string, string[]> localLines, Type cl)
    {
        try
        {
            string text = "";
            if (stringSubFolder != "")
            {
                text = stringSubFolder + "/";
            }
            if (Util.PackManager().GetStrings(text + cl.ToString()) != null)
            {
                return Util.PackManager().GetStrings(text + cl.ToString());
            }
            return localLines;
        }
        catch
        {
            return localLines;
        }
    }

    public virtual Dictionary<string, string[]> GetDefaultStrings()
    {
        return null;
    }

    public string[] GetStringArray(string key)
    {
        if (strings.ContainsKey(key))
        {
            return strings[key];
        }
        return new string[1] { "* ERROR" };
    }

    public string GetString(string key, int index)
    {
        if (strings.ContainsKey(key))
        {
            string[] array = strings[key];
            if (index < array.Length && index >= 0)
            {
                return array[index];
            }
        }
        return "ERROR";
    }

    public bool StringArrayExists(string key)
    {
        return strings.ContainsKey(key);
    }

    public bool StringExists(string key, int index)
    {
        if (strings.ContainsKey(key))
        {
            return index < strings[key].Length;
        }
        return false;
    }

    protected void SetStrings(Dictionary<string, string[]> setStrings, Type classType)
    {
        strings = GetStrings(setStrings, classType);
    }
}
