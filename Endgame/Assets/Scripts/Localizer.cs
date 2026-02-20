using System.Collections.Generic;
using UnityEngine;

public static class Localizer
{
    public static string GetGlobalFont()
    {
        return "DTM-Sans";
    }

    public static string[] FormatArray(string[] strings, params object[] vars)
    {
        List<string> list = new List<string>();
        foreach (string format in strings)
        {
            list.Add(string.Format(format, vars));
        }
        return list.ToArray();
    }
}
