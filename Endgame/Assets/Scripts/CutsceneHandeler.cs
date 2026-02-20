using System;
using UnityEngine;

public class CutsceneHandler : UnityEngine.Object
{
    private static Type[] cutscenes = new Type[2]
    {
        typeof(KnightDetectHMCutscene),
        typeof(KingAtCastle),
    };

    public static CutsceneBase GetCutscene(int id)
    {
        Debug.Log(id);
        return new GameObject("Cutscene", cutscenes[id]).GetComponent<CutsceneBase>();
    }

    public static int GetCutsceneCount()
    {
        return cutscenes.Length;
    }
}
