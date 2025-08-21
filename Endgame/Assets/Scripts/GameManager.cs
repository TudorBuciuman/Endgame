using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class GameManager : MonoBehaviour
{
    public static GameManager instance=null;

    public SaveFile savedFile;

    public SaveFile currentFile;

    private bool menuDisabled;

    private bool menuLocked;

    private GameObject menu;

    private bool menuIsOpen;

    private string playerName;

    private List<int> items;

    private int weapon;

    private int armor;

    private int hp;

    private int deaths;

    private int exp;

    private int gold;

    public bool canMove;

    public bool canInteract;


    private readonly int[] lvs = new int[20]
    {
        0, 10, 30, 70, 120, 200, 300, 500, 800, 1200,
        1700, 2500, 3500, 5000, 7000, 10000, 15000, 25000, 50000, 99999
    };

    private int atBuffs;

    private int dfBuffs;

    private bool susieActive = true;

    private bool noelleActive = true;

    private int miniPartyMember = -1;

    private int zone;

    private int oldZone;

    private bool lastZoneForceLoad = true;

    private Vector2 spawnPos;

    private Vector2 spawnDir;

    private bool savePointSpawn;

    private bool newSceneFadeIn;

    private bool wrongWarp;


    private MusicPlayer mp;

    private AudioSource aud;

    private int healAudFrames;

    private string healAudSound = "sounds/snd_heal";

    private string nextOWSong;

    private bool checkpointEnabled;

    private Vector3 checkpointPos = Vector3.zero;

    private int forceRespawnZone = -1;

    private int battleId;

    private int battleEndState;

    //public Config config;

    private PackManager packManager;

    //private MiscellaneousStrings miscStrings;


    public void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Application.targetFrameRate = 30;
        QualitySettings.vSyncCount = 0;
        canMove = true;
        canInteract = true;
        menuIsOpen = false;
        menuDisabled = false;
        menuLocked = false;
        items = new List<int>(){0, 1, 23, 34,15,2,15,-1};
        armor = 7;
        weapon = 4;
        exp = -1;
        if (instance==null)
        {
            instance = this;
            GameObject gameObject = new GameObject("FadeCanvas", typeof(Canvas));
            gameObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            //gameObject.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
            gameObject.GetComponent<Canvas>().sortingOrder = 2000;
            gameObject.transform.position = Vector3.zero;
            gameObject.transform.localScale = new Vector3(1f / 48f, 1f / 48f, 1f);
            Instantiate(Resources.Load<GameObject>("ui/FadeObj"), gameObject.transform).name = "FadeObj";
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(this);

            mp = base.gameObject.AddComponent<MusicPlayer>();
            aud = base.gameObject.AddComponent<AudioSource>();

            packManager = base.gameObject.AddComponent<PackManager>();
            //config = new Config("config.ini");
            //LoadConfigData();
            base.gameObject.AddComponent<UTInput>();
            // miscStrings = base.gameObject.AddComponent<MiscellaneousStrings>();
            GameObject obj = Instantiate(Resources.Load<GameObject>("ui/QuitFunction"));
            obj.name = "QuitFunction";
            UnityEngine.Object.DontDestroyOnLoad(obj);
        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && !menuIsOpen && !menuDisabled && !menuLocked && canInteract)
        {
            menu = new GameObject();
            menuIsOpen = true;
            canMove = false;
            canInteract = false;
            menu.AddComponent<MainMenu>().CreateMainMenu();
        }
    }
    public string GetPlayerName()
    {
        return playerName;
    }
    public void PlayTimedHealSound()
    {
        healAudFrames = 1;
        healAudSound = "sounds/snd_heal";
    }

    public void PlayGlobalSFX(string clip)
    {
        aud.clip = Resources.Load<AudioClip>(clip);
        aud.Play();
    }
    public int GetATK()
    {
        int num = Items.ItemValue(GetWeapon());
        return GetATKRaw() + num;
    }

    public int GetATKRaw()
    {
        int num = (GetLV() - 1) * 2;
        //num += atBuffs;
        return num;
    }

    public int GetDEF()
    {
        return GetDEFRaw() + Items.ItemValue(GetArmor());
    }

    public int GetDEFRaw()
    {
        int num = Mathf.FloorToInt((float)GetLV() / 5f);
        //num += dfBuffs;
        return num;
    }

    public void PlayMusic(string music, float pitch, float volume)
    {
        if (music == "zoneMusic" && FindFirstObjectByType<CameraController>())
        {
            //music = UnityEngine.Object.FindObjectOfType<CameraController>().GetZoneMusic();
            
            pitch = FindFirstObjectByType<CameraController>().GetZoneMusicPitch();
            /*
            if ((int)GetFlag(87) >= 5 && music == "music/mus_happyhappy")
            {
                pitch = 0.3f;
            }
            if ((int)GetFlag(87) >= 5 && music == "music/mus_twoson_intro")
            {
                music = "music/mus_birdnoise";
            }
            */
        }
        /*
        if (music.EndsWith("mus_snowy"))
        {
            pitch = ((zone >= 50 && zone < 110) ? 0.475f : (((int)GetFlag(13) >= 3) ? 0.6f : 0.95f));
        }
        if (music.EndsWith("mus_muscle") && playerName == "SHAYY" && (zone != 115 || GetFlagInt(291) == 0))
        {
            music = "music/mus_muscle_improved";
        }
        */
        /*
        bool intro = false;
        if (music.EndsWith("_intro"))
        {
            intro = true;
            music = music.Replace("_intro", "");
        }
        mp.SetVolume(volume);
        if ((mp.CurrentMusic() != music || !mp.IsPlaying()) && music != "" && music != "music/")
        {
            mp.ChangeMusic(music, intro, playImmediately: true);
            mp.GetSource().pitch = pitch;
        }
        else if (music == "")
        {
            mp.Stop();
        }
        */
    }

    public string GetPlayingMusic()
    {
        return mp.CurrentMusic();
    }

    public void PlayMusic(string music, float pitch)
    {
        PlayMusic(music, pitch, 1f);
    }

    public void PlayMusic(string music)
    {
        PlayMusic(music, 1f);
    }

    public void StopSFX()
    {
        aud.Stop();
    }

    public bool IsMenuOpen()
    {
        return menuIsOpen;
    }
    public void SetMenu(bool open)
    {
        menuIsOpen = open;
    }
    public void StopMusic()
    {
        if ((bool)mp)
        {
            mp.Stop();
        }
    }

    public void StopMusic(float fadeOutFrames)
    {
        if ((bool)mp)
        {
            if (fadeOutFrames <= 0f)
            {
                StopMusic();
            }
            else
            {
                mp.FadeOut(fadeOutFrames / 30f);
            }
        }
    }

    public void PauseMusic()
    {
        if ((bool)mp)
        {
            mp.Pause();
        }
    }

    public void ResumeMusic()
    {
        if ((bool)mp)
        {
            mp.Resume();
        }
    }

    public void ResumeMusic(int fadeInFrames)
    {
        if ((bool)mp && mp.IsPaused())
        {
            ResumeMusic();
            if (fadeInFrames > 0)
            {
                mp.FadeIn((float)fadeInFrames / 30f);
            }
        }
    }
    public void ForceLoadArea(int sceneName)
    {
        lastZoneForceLoad = true;
        nextOWSong = "zoneMusic";
        zone = sceneName;
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        SceneManager.sceneLoaded += OnAreaLoaded;
    }

    public void LoadArea(int sceneName, bool fadeIn, Vector2 pos, byte dir)
    {
        lastZoneForceLoad = false;
        nextOWSong = "zoneMusic";
        zone = sceneName;
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        spawnPos = pos;
        if (dir == 0)
        {
            //up
            spawnDir = new Vector2(0, 1);
        }
        else if (dir == 1)
        {
            //right
            spawnDir = new Vector2(1, 0);
        }
        else if (dir == 2)
        {
            //left
            spawnDir = new Vector2(0,-1);
        }
        else
        {
            //down
            spawnDir = new Vector2(-1, 0);
        }
        newSceneFadeIn = fadeIn;
        SceneManager.sceneLoaded += OnAreaLoaded;
    }

    private void OnAreaLoaded(Scene ascene, LoadSceneMode aMode)
    {
        SceneManager.sceneLoaded -= OnAreaLoaded;
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(zone));
            GameObject.Find("Canvas").GetComponent<Canvas>().pixelPerfect = true;
            //EnableMenu();
            GameObject gameObject = GameObject.Find("FadeObj");
            if (newSceneFadeIn)
            {
                gameObject.GetComponent<Fade>().FadeIn(13);
            }
            if ((bool)GameObject.Find("Player") && !lastZoneForceLoad)
            {
                if (savePointSpawn && !checkpointEnabled)
                {
                //WTF?    
                //spawnPos = UnityEngine.Object.FindObjectOfType<SAVEPoint>().GetSpawnPosition();
                }
                else if (savePointSpawn && checkpointEnabled)
                {
                    if (checkpointPos == Vector3.zero)
                    {
                        spawnPos = GameObject.Find("Player").transform.position;
                    }
                    else
                    {
                        spawnPos = checkpointPos;
                    }
                    spawnDir = Vector2.down;
                    //UnlockMenu();
                }
                if (wrongWarp)
                {
                    spawnPos = GameObject.Find("Player").transform.position;
                    spawnDir = Vector2.down;
                    wrongWarp = false;
                }
                if ((bool)GameObject.Find("Player").GetComponent<PlayerController>())
                {
                    GameObject.Find("Player").GetComponent<PlayerController>().HandleSpawn(spawnPos, spawnDir);
                }
            }
            savePointSpawn = false;
            //EnablePlayerMovement();
            //PlayMusic(nextOWSong);
    }
    public int GetLV()
    {
        return GetLV(exp);
    }

    public int GetLV(int exp)
    {
        if (exp < 0)
        {
            return 1;
        }
        for (int i = 0; i < lvs.Length; i++)
        {
            if (exp < lvs[i])
            {
                return i;
            }
        }
        return lvs.Length;
    }

    public int GetLVExp()
    {
        return GetExpForLV(GetLV() + 1);
    }

    public int GetExpForLV(int lv)
    {
        if (lv > 0 && lv <= lvs.Length)
        {
            return lvs[lv - 1];
        }
        return lvs[lvs.Length - 1];
    }

    public void AddEXP(int exp)
    {
        this.exp += exp;
    }

    public void SetEXP(int exp)
    {
        this.exp = exp;
    }

    public int GetEXP()
    {
        return exp;
    }

    public int GetGold()
    {
        return gold;
    }

    public void AddGold(int gold)
    {
        this.gold += gold;
    }

    public void RemoveGold(int gold)
    {
        this.gold -= gold;
        if (this.gold < 0)
        {
            this.gold = 0;
        }
    }

    public void SetGold(int gold)
    {
        this.gold = gold;
    }
    public int GetHP()
    {
        return hp;
    }
    public int GetMaxHP()
    {
        return GetMaxHP(exp);
    }

    public int GetMaxHP(int exp)
    {
        float num = 1;
        float num2 = 1;
        int num3 = Mathf.RoundToInt(20f * num + (float)(4 * (GetLV(exp) - 1)) * num2);
        if (GetLV(exp) == 20)
        {
            num3 = 99;
        }

        return num3;
    }
    public int GetItem(int id)
    {
        return items[id];
    }

    public int GetWeapon()
    {
        return weapon;
    }

    public int GetArmor()
    {
        return armor;
    }
    public void UseItem(int index)
    {
        if (Items.ItemType(GetItem(index)) == 0)
        {
            PlayGlobalSFX("sounds/snd_heal");

            EatItem(index);
        }
        else if (Items.ItemType(GetItem(index)) == 1)
        {
            PlayGlobalSFX("sounds/snd_item");
            aud.Play();
            ChangeWeapon(index);
        }
        else if (Items.ItemType(GetItem(index)) == 4)
        {
            healAudFrames = 1;
            healAudSound = "sounds/snd_heal";
            int heal = Items.ItemValue(GetItem(index));
            //HealAll(heal, includeOutOfParty: false);
            RemoveItem(index);
        }
    }
    public void RemoveItem(int index)
    {
        items.RemoveAt(index);
        items.Add(-1);
    }
    public void SetMenuDis()
    {
        menuDisabled = true;
    }
    public void ChangeWeapon(int index)
    {
        int id = weapon;
        weapon = items[index];
        RemoveItem(index);
        AddItem(id);
    }
    public void AddItem(int id)
    {
        if (id > -1)
        {
            items[FirstFreeItemSpace()] = id;
        }
    }
    public int FirstFreeItemSpace()
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == -1)
            {
                return i;
            }
        }
        return -1;
    }
    public void ChangeArmor(int index)
    {
        int num = armor;
        armor = items[index];
        RemoveItem(index);
        AddItem(num);
    }

    public void EatItem(int index)
    {
        int item = GetItem(index);
        int heal = Items.ItemValue(item);
        Heal(heal);
        
        RemoveItem(index);
    }

    public void Heal(int heal)
    {
        if (hp <= GetMaxHP())
        {
            hp += heal;
            if (hp> GetMaxHP())
            {
                hp= GetMaxHP();
            }
        }
    }

    public void SetHP(int hp)
    {
        if (hp > GetMaxHP())
        {
            hp = GetMaxHP();
        }
        if (hp <= 0)
        {
            hp = 0;
            //Death();
        }
    }
    public bool MenuDis()
    {
        return menuDisabled;
    }
    public void SetMenuDisabled(bool v)
    {
        menuDisabled = v;
    }
}
