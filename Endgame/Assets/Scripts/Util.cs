using System;
using System.Text.RegularExpressions;
using UnityEngine;

public static class Util
{
    public static GameManager GameManager()
    {
        if ((bool)global::GameManager.instance)
        {
            return global::GameManager.instance;
        }
        return UnityEngine.Object.FindObjectOfType<GameManager>();
    }

    public static PackManager PackManager()
    {
        if ((bool)global::PackManager.instance)
        {
            return global::PackManager.instance;
        }
        return UnityEngine.Object.FindObjectOfType<PackManager>();
    }

    /*
    public static MiscellaneousStrings MiscStrings()
    {
        return UnityEngine.Object.FindObjectOfType<MiscellaneousStrings>();
    }
    */
    public static string Unescape(string str)
    {
        try
        {
            string text = "";
            for (int i = 0; i < str.Length; i++)
            {
                text += ((i > 0 && str[i - 1] == '\\') ? char.ToLower(str[i]) : str[i]);
            }
            return Regex.Unescape(text);
        }
        catch (ArgumentException ex)
        {
            Debug.LogWarning(ex);
            return (ex is ArgumentNullException) ? "* [NULL_STRING]" : "* [INVALID_ESCAPE]";
        }
        catch (IndexOutOfRangeException message)
        {
            Debug.LogWarning(message);
            return "* [INVALID_STRING]";
        }
    }

    public static string BattleHUDFontFix(string text)
    {
        string text2 = "";
        for (int i = 0; i < text.Length; i++)
        {
            text2 = ((char.ToLower(text[i]) != 'u') ? ((char.ToLower(text[i]) != 'v') ? (text2 + text[i]) : (text2 + "u")) : (text2 + "v"));
        }
        return text2;
    }
}
