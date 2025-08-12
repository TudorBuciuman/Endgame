using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PackManager : MonoBehaviour
{
    public static PackManager instance;

    private AssetBundle currentPack;

    private string packName = "";

    private static Type[] validAssets = new Type[4]
    {
        typeof(TextAsset),
        typeof(Texture2D),
        typeof(Sprite),
        typeof(Font)
    };

    private string packsFolder = "";

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            packsFolder = Path.Combine(Directory.GetParent(Application.dataPath).ToString(), "lang/");
        }
        else
        {
            UnityEngine.Object.Destroy(this);
        }
    }

    public void UpdateQuitSprites()
    {
        /*
        int num = 0;
        Sprite[] sprites = UnityEngine.Object.FindObjectOfType<QuitFunction>().GetSprites();
        Sprite[] array = sprites;
        foreach (Sprite localSprite in array)
        {
            string spr = "ui/spr_quitmessage_" + num;
            sprites[num] = GetTranslatedSprite(localSprite, spr);
            num++;
        }
        UnityEngine.Object.FindObjectOfType<QuitFunction>().SetSprites(sprites);
        */
    }

    public static AssetBundle LoadBundle(string filename)
    {
        if (filename == null || filename == "")
        {
            return null;
        }
        if (File.Exists(filename))
        {
            try
            {
                return AssetBundle.LoadFromFile(filename);
            }
            catch
            {
                Debug.LogWarning("Invalid asset pack tried to load " + filename);
                return null;
            }
        }
        return null;
    }

    public static UnityEngine.Object LoadBundleAsset(AssetBundle bundle, string name)
    {
        UnityEngine.Object @object = bundle.LoadAsset(name);
        if ((bool)@object)
        {
            Type[] array = validAssets;
            foreach (Type type in array)
            {
                if (@object.GetType() == type)
                {
                    return @object;
                }
            }
        }
        Debug.LogWarning("Invalid asset tried/failed to load " + name);
        UnityEngine.Object.DestroyImmediate(@object);
        return null;
    }

    public static UnityEngine.Object LoadBundleSubAsset(AssetBundle bundle, string name, int id = 0)
    {
        UnityEngine.Object @object = bundle.LoadAssetWithSubAssets(name)[id];
        if ((bool)@object)
        {
            Type[] array = validAssets;
            foreach (Type type in array)
            {
                if (@object.GetType() == type)
                {
                    return @object;
                }
            }
        }
        Debug.LogWarning("Invalid asset tried/failed to load " + name);
        UnityEngine.Object.DestroyImmediate(@object);
        return null;
    }

    public Dictionary<string, string[]> GetStrings(string file)
    {
        string text = "Assets/pack/text/" + file + ".json";
        if (currentPack != null && currentPack.Contains(text))
        {
            TextAsset textAsset = (TextAsset)LoadBundleAsset(currentPack, text);
            if ((bool)textAsset)
            {
                return (Dictionary<string, string[]>)Serializer.Deserialize(typeof(Dictionary<string, string[]>), textAsset.ToString());
            }
        }
        return null;
    }

    public T GetSerializedClass<T>(string file)
    {
        string text = "Assets/pack/" + file + ".json";
        if (currentPack != null && currentPack.Contains(text))
        {
            TextAsset textAsset = (TextAsset)LoadBundleAsset(currentPack, text);
            if ((bool)textAsset)
            {
                return (T)Serializer.Deserialize(typeof(T), textAsset.ToString());
            }
        }
        return default(T);
    }

    public Sprite GetTranslatedSprite(Sprite localSprite, string spr)
    {
        string text = "Assets/pack/sprites/" + spr + ".png";
        if (currentPack != null && currentPack.Contains(text))
        {
            Sprite sprite = (Sprite)LoadBundleSubAsset(currentPack, text, 1);
            if ((bool)sprite)
            {
                return sprite;
            }
        }
        return localSprite;
    }

    public AudioClip GetTranslatedAudio(AudioClip localAudio, string aud)
    {
        string text = "Assets/pack/sounds/voice/" + aud + ".wav";
        if (currentPack != null && currentPack.Contains(text))
        {
            AudioClip audioClip = (AudioClip)LoadBundleAsset(currentPack, text);
            if ((bool)audioClip)
            {
                return audioClip;
            }
        }
        return localAudio;
    }

    public Font GetFont(Font localFont, string fnt)
    {
        string text = "Assets/pack/fonts/" + fnt;
        if (currentPack != null)
        {
            if (currentPack.Contains(text + ".ttf"))
            {
                Font font = (Font)LoadBundleAsset(currentPack, text + ".ttf");
                if ((bool)font)
                {
                    return font;
                }
            }
            else if (currentPack.Contains(text + ".otf"))
            {
                Font font2 = (Font)LoadBundleAsset(currentPack, text + ".otf");
                if ((bool)font2)
                {
                    return font2;
                }
            }
        }
        return localFont;
    }

    public void ReplaceTextBoxFonts()
    {
        Text[] array = UnityEngine.Object.FindObjectsOfType<Text>();
        foreach (Text text in array)
        {
            text.font = GetFont(Resources.Load<Font>("fonts/" + text.font.name), text.font.name);
        }
    }

    public void SceneLoaded(Scene ascene, LoadSceneMode aMode)
    {
        ReplaceTextBoxFonts();
    }

    public void SetPack(string name)
    {
        if (currentPack != null)
        {
            currentPack.Unload(unloadAllLoadedObjects: true);
        }
        if (name != "")
        {
            if (File.Exists(Path.Combine(packsFolder, name + ".pack")))
            {
                currentPack = LoadBundle(Path.Combine(packsFolder, name + ".pack"));
                if ((bool)currentPack)
                {
                    packName = name;
                }
            }
            else
            {
                packName = "";
            }
        }
        else
        {
            packName = "";
        }
        UpdateQuitSprites();
    }
    /*
    public LanguagePack[] GetPacks(bool reloadPack)
    {
        string pack = GetPackName();
        SetPack("");
        List<LanguagePack> list = new List<LanguagePack>();
        LanguagePack item = new LanguagePack(null, "");
        list.Add(item);
        if (Directory.Exists(packsFolder))
        {
            string[] files = Directory.GetFiles(packsFolder, "*.pack", SearchOption.TopDirectoryOnly);
            foreach (string text in files)
            {
                AssetBundle assetBundle = LoadBundle(text);
                if ((bool)assetBundle)
                {
                    list.Add(new LanguagePack(assetBundle, text));
                }
            }
        }
        SetPack(pack);
        return list.ToArray();
    }
    */
    /*
    public static void DebugPrintPackInfo(LanguagePack[] packs)
    {
        foreach (LanguagePack obj in packs)
        {
            MonoBehaviour.print(obj.GetFileName());
            MonoBehaviour.print(obj.GetPackInfo().language);
            MonoBehaviour.print(obj.GetPackInfo().description);
        }
    }
    */
    public string GetPackName()
    {
        return packName;
    }

    public AssetBundle GetCurrentPack()
    {
        return currentPack;
    }

    private void OnApplicationQuit()
    {
        AssetBundle.UnloadAllAssetBundles(unloadAllObjects: true);
    }
}
