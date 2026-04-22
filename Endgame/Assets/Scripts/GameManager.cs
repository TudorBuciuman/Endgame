using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.XR;

public class GameManager : MonoBehaviour
{
    public static GameManager instance=null;

    public SAVEFile save;

    public SAVEFile currentSave;

    public SAVEFile checkpointSave;

    private object[] flags;

    private object[] persFlags;

    private object[] sessionFlags;

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

    private int scene;

    public bool canMove;

    public bool canInteract;

    public static bool test = false;
    public static bool mobile = false;
    private readonly int[] lvs = new int[20]
    {
        0, 10, 30, 70, 120, 200, 300, 500, 800, 1200,
        1700, 2500, 3500, 5000, 7000, 10000, 15000, 25000, 50000, 99999
    };
    private int zone;

    private int oldZone;

    private bool lastZoneForceLoad = true;

    private Vector2 spawnPos;

    private Vector2 spawnDir;

    private bool savePointSpawn;

    private bool newSceneFadeIn;

    private bool wrongWarp;

    private bool trackTime;
    private float playTimeFrames;
    private int playTime;

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

    private PackManager packManager;

    private bool fullscreen = true;

    public void Awake()
    {
        if(GetFlagInt(66) == 1)
        {
            test = true;
        }
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 1;
        
        canMove = true;
        canInteract = true;
        menuIsOpen = false;
        menuDisabled = false;
        menuLocked = false;
        if (instance==null)
        {
#if UNITY_EDITOR
    test = true;
#endif
#if UNITY_ANDROID
    mobile=true;
    Instantiate(Resources.Load<GameObject>("ui/MobileUI"));
    FindFirstObjectByType<MobileUI>().EnableButtons(dPadEnabled: true, z: true, x: true, c: true, instant: false);
#endif
            if (!mobile)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            instance = this;
            SetDefaultValues();
            GameObject gameObject = new GameObject("FadeCanvas", typeof(Canvas));
            gameObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            gameObject.GetComponent<Canvas>().sortingOrder = 2000;
            gameObject.transform.position = Vector3.zero;
            gameObject.transform.localScale = new Vector3(1f / 48f, 1f / 48f, 1f);
            Instantiate(Resources.Load<GameObject>("ui/FadeObj"), gameObject.transform).name = "FadeObj";
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(this);
            if (test)
            {
                items = new List<int> { 17, 5, 24, 25, 26, 27, 28, 29 };
                SetFlag(66, 1);
            }
            mp = base.gameObject.AddComponent<MusicPlayer>();
            aud = base.gameObject.AddComponent<AudioSource>();

            packManager = base.gameObject.AddComponent<PackManager>();
            base.gameObject.AddComponent<UTInput>();
            GameObject obj = Instantiate(Resources.Load<GameObject>("ui/QuitFunction"));
            obj.name = "QuitFunction";
            DontDestroyOnLoad(obj);
            trackTime = false;
        }
        else if (instance != this)
        {
            instance.menuDisabled = false;
            Destroy(this.gameObject);
        }
    }
    private void Start()
    {
        if (FindFirstObjectByType<PlayerController>() != null)
        {
            PlayMusic(FindFirstObjectByType<CameraController>().GetZoneMusic(), FindFirstObjectByType<CameraController>().GetZoneMusicPitch());
            StartTime();
        }
    }
    private void Update()
    {
        if (UTInput.GetButtonDown("C") && !menuIsOpen && !menuDisabled && !menuLocked && canInteract)
        {
            menu = new GameObject();
            menuIsOpen = true;
            canMove = false;
            canInteract = false;
            menu.AddComponent<MainMenu>().CreateMainMenu();
        }
        if (trackTime)
        {
            playTimeFrames+=Time.deltaTime;
            if (playTimeFrames>1)
            {
                playTime++;
                playTimeFrames -= 1;
            }
        }
        if (Input.GetKeyDown(KeyCode.F4) && test)
        {
            fullscreen = !fullscreen;
            if (fullscreen)
            {
                Resolution currentResolution = Screen.currentResolution;
                Screen.SetResolution(currentResolution.width, currentResolution.height, FullScreenMode.FullScreenWindow);
            }
            else
            {
                Screen.SetResolution(640, 480, fullscreen: false);
            }
        }
    }
    public void StartTrackTime()
    {
        trackTime = true;
    }
    public void SetDefaultValues()
    {
        playerName = "Pawn";
        items = new List<int> { -1, -1, -1, -1, -1, -1, -1, -1 };
        weapon = 0;
        armor = 16;
        hp = 20;
        deaths = 0;
        gold = 10;
        scene = 1;
        exp = 0;
        zone = 6;
        playTime = 0;
        playTimeFrames = 0;
        flags = new object[1000];
        persFlags = new object[1000];
        sessionFlags = new object[100];
        save.pos = new Vector2(-7.885f, -1);
        SetFlag(0, "neutral");
        SetFlag(1, "neutral");
        SetFlag(2, "neutral");
        SetFlag(12, 1);
        SetSessionFlag(11, 2);
        if (File.Exists(Path.Combine(Application.persistentDataPath, "Save.sav")))
        {
            LoadFile();
        }
        checkpointSave = save;
        menuLocked = false;
    }
    public int GetZone()
    {
        return zone;
    }
    public void LoadFile()
    {
        string path = "SAVE.sav";
        using (FileStream fs = File.Open(Path.Combine(Application.persistentDataPath, path), FileMode.Open))
        {
            SAVEFileIO.ReadFile(ref save, fs);
        }
        items = save.items;
        weapon = save.weapon;
        armor = save.armor;
        hp = GetMaxHP(save.exp);
        deaths = save.deaths;
        gold = save.gold;
        scene = save.scene;
        exp = save.exp;
        zone = save.zone;
        playTime = save.playTime;
        playTimeFrames = 0;
        flags = save.flags;
        persFlags = save.persFlags;
        sessionFlags = save.persFlags;
        SetFlag(12, 1);
        SetSessionFlag(11, 2);
    }
    public SAVEFile GetFile()
    {
        SAVEFile sAVEFile = new SAVEFile();
        sAVEFile.UpdateCharacterInfo(playerName, exp, items, weapon, armor, playTime, zone, gold,scene, FindFirstObjectByType<PlayerController>().gameObject.transform.localPosition,"[???]", flags);
        sAVEFile.UpdatePersistentFlags(persFlags);
        sAVEFile.UpdateDeathCount(deaths);
        return sAVEFile;
    }
    public void SpawnFromLastSave(bool respawn)
    {
        if (!respawn)
        {
            sessionFlags = new object[100];
        }
        else
        {
            //flags = (object[])checkpointSave.flags.Clone();
            exp = checkpointSave.exp;
            hp = GetMaxHP(GetEXP());
            playerName = checkpointSave.name;
            items = new List<int>(checkpointSave.items);
            weapon = (int)checkpointSave.weapon;
            armor = (int)checkpointSave.armor;
            playTime = checkpointSave.playTime;
            zone = checkpointSave.zone;
            gold = checkpointSave.gold;
            if (forceRespawnZone > -1)
            {
                zone = forceRespawnZone;
                forceRespawnZone = -1;
            }
            StartTime();
            LoadArea(zone, fadeIn: true, checkpointSave.pos, Vector2.down, true);
            return;
        }
        exp = 0;
        hp = GetMaxHP(0);
        zone = 6;
        gold = 10;
        StartTime();
        if (!respawn)
        {
            deaths = save.deaths;
            persFlags = (object[])save.persFlags.Clone();
        }
        LoadArea(zone, respawn, new Vector2(-7.885f,-1f), Vector2.down, fromSavePoint: true);
    }
    public bool FileExists()
    {
        string path = "SAVE.sav";
        return File.Exists(Path.Combine(Application.persistentDataPath, path));
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
    public int GetAct()
    {
        return scene;
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
            music = GameObject.Find("Camera").GetComponent<CameraController>().GetZoneMusic();
            
            pitch = GameObject.Find("Camera").GetComponent<CameraController>().GetZoneMusicPitch();
        }
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
    }
    public void PlayMusic(AudioClip clip)
    {
        mp.ChangeMusic(clip, false, true, false, 0);
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
    public void SetMenuToBeOpened()
    {
        canMove = true;
        canInteract = true;
        menuDisabled = false;
        menuIsOpen = false;
        menuLocked = false;
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
    public void LoadArea(int sceneName, bool fadeIn, Vector2 pos, Vector2 dir, bool fromSavePoint)
    {
        LoadArea(sceneName, fadeIn, pos, dir);
        savePointSpawn = fromSavePoint;
        EnablePlayerMovement();
    }
    public void LoadArea(int sceneName, bool fadeIn, Vector2 pos, Vector2 dir)
    {
        if (FindFirstObjectByType<PlayerController>())
        {
            FindFirstObjectByType<PlayerController>().SetCollision(onoff: true);
        }
        DisablePlayerMovement(true);
        lastZoneForceLoad = false;
        nextOWSong = "zoneMusic";
        zone = sceneName;
        if(GameObject.Find("Camera").GetComponent<CameraController>())
        GameObject.Find("Camera").GetComponent<CameraController>().SetFollowPlayer(false);

        currentsc = SceneManager.GetActiveScene().buildIndex;
        if (GameObject.Find("Player"))
            GameObject.Find("Player").name = "player";
        if (GameObject.Find("CameraBound_0"))
            GameObject.Find("CameraBound_0").name = "CB0";
        if (GameObject.Find("CameraBound_1"))
            GameObject.Find("CameraBound_1").name = "CB1";
        if (GameObject.Find("Canvas"))
            GameObject.Find("Canvas").name = "canvas";
        if (GameObject.Find("Camera"))
            GameObject.Find("Camera").name = "camera";
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        spawnPos = pos;
        spawnDir = dir;
        newSceneFadeIn = fadeIn;
        SceneManager.sceneLoaded += OnAreaLoaded;
    }
    int currentsc;
    public void LoadArea(int sceneName, bool fadeIn, Vector2 pos, byte dir)
    {
        lastZoneForceLoad = false;
        nextOWSong = "zoneMusic";
        zone = sceneName;
        currentsc = SceneManager.GetActiveScene().buildIndex;
        if (FindFirstObjectByType<CameraController>())
        {
            GameObject.Find("Camera").GetComponent<CameraController>().SetFollowPlayer(false);
            if (GameObject.Find("Player"))
                GameObject.Find("Player").name = "player";
            if (GameObject.Find("CameraBound_0"))
                GameObject.Find("CameraBound_0").name = "CB0";
            if (GameObject.Find("CameraBound_1"))
                GameObject.Find("CameraBound_1").name = "CB1";
            if (GameObject.Find("Canvas"))
                GameObject.Find("Canvas").name = "canvas";
            if (GameObject.Find("Camera"))
                GameObject.Find("Camera").name = "camera";
            DisablePlayerMovement(true);
        }
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
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
    public void InstantFade(int time)
    {
        GameObject gameObject = GameObject.Find("FadeObj");
        gameObject.GetComponent<Fade>().FadeOut(time);
    }
    private void OnAreaLoaded(Scene ascene, LoadSceneMode aMode)
    {
        SceneManager.sceneLoaded -= OnAreaLoaded;
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(zone));
        SceneManager.UnloadSceneAsync(currentsc);
        GameObject.Find("Canvas").GetComponent<Canvas>().pixelPerfect = true;
        GameObject gameObject = GameObject.Find("FadeObj");
        if (newSceneFadeIn)
        {
            gameObject.GetComponent<Fade>().FadeIn(30);
        }
        if ((bool)GameObject.Find("Player") && !lastZoneForceLoad)
        {
            if (savePointSpawn && checkpointEnabled)
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
        PlayMusic(nextOWSong);
        EnablePlayerMovement();
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
            if(FindFirstObjectByType<MainMenu>())
            FindFirstObjectByType<MainMenu>().RewriteHealth();
        }
        else if (Items.ItemType(GetItem(index)) == 1)
        {
            PlayGlobalSFX("sounds/snd_item");
            aud.Play();
            ChangeWeapon(index);
        }
        else if (Items.ItemType(GetItem(index)) == 2)
        {
            PlayGlobalSFX("sounds/snd_item");
            aud.Play();
            ChangeArmor(index);
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
        if (FindFirstObjectByType<PartyPanels>())
        {
            FindFirstObjectByType<PartyPanels>().UpdateHP(hp);
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
    public object GetFlag(int i)
    {
        if (flags == null || i < 0 || i > flags.Length || flags[i] == null)
        {
            return 0;
        }
        return flags[i];
    }

    public int GetFlagInt(int i)
    {
        return (int)GetFlag(i);
    }

    public string GetFlagString(int i)
    {
        return GetFlag(i).ToString();
    }

    public double GetFlagDouble(int i)
    {
        return (double)GetFlag(i);
    }

    public void SetFlag(int i, object state)
    {
        //UnityEngine.Debug.LogFormat("SetFlag({0}, {1})", i, state);
        if (i >= 0 && i <= flags.Length)
        {
            flags[i] = state;
        }
    }
    public object GetPersistentFlag(int i)
    {
        if (persFlags == null || i < 0 || i > persFlags.Length || persFlags[i] == null)
        {
            return 0;
        }
        return persFlags[i];
    }

    public int GetPersistentFlagInt(int i)
    {
        return (int)GetPersistentFlag(i);
    }

    public string GetPersistentFlagString(int i)
    {
        return GetPersistentFlag(i).ToString();
    }

    public double GetPersistentFlagDouble(int i)
    {
        return (double)GetPersistentFlag(i);
    }

    public void SetSessionFlag(int i, object state)
    {
        sessionFlags[i] = state;
    }

    public object GetSessionFlag(int i)
    {
        if (sessionFlags == null || sessionFlags[i] == null)
        {
            return 0;
        }
        return sessionFlags[i];
    }

    public int GetSessionFlagInt(int i)
    {
        return (int)GetSessionFlag(i);
    }

    public string GetSessionFlagString(int i)
    {
        return GetSessionFlag(i).ToString();
    }

    public double GetSessionFlagDouble(int i)
    {
        return (double)GetSessionFlag(i);
    }
    public int NumItemFreeSpace()
    {
        int num = 0;
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == -1)
            {
                num++;
            }
        }
        return num;
    }
    public void StartTime()
    {
        trackTime = true;
    }

    public int GetCurrentZone()
    {
        return zone;
    }
    public string GetFormattedPlayTime()
    {
        if (FileExists())
        {
            string text = Mathf.FloorToInt((float)save.playTime / 60f).ToString();
            string text2 = (save.playTime % 60).ToString();
            if (text2.Length == 1)
            {
                text2 = "0" + text2;
            }
            return text + ":" + text2;
        }
        return "0:00";
    }
    public string GetCurrentPlayTime()
    {
        string text = Mathf.FloorToInt((float)playTime / 60f).ToString();
        string text2 = (playTime % 60).ToString();
        if (text2.Length == 1)
        {
            text2 = "0" + text2;
        }
        return text + ":" + text2;
    }
    public string GetFormattedPlayTimeFromTime(int playTime)
    {
        string text = Mathf.FloorToInt((float)playTime / 60f).ToString();
        string text2 = (playTime % 60).ToString();
        if (text2.Length == 1)
        {
            text2 = "0" + text2;
        }
        return text + ":" + text2;
    }
    public string GetFormattedUpdatedPlayTime()
    {
        string text = Mathf.FloorToInt((float)playTime / 60f).ToString();
        string text2 = (playTime % 60).ToString();
        if (text2.Length == 1)
        {
            text2 = "0" + text2;
        }
        return text + ":" + text2;
    }
    public void SaveFile(bool savepoint)
    {
        SetFlag(177, Application.version);
        zone = SceneManager.GetActiveScene().buildIndex;
        if (savepoint)
        {
            DeactivateCheckpoint();
            save.UpdateCharacterInfo(playerName, exp, items, weapon, armor, playTime, zone, gold, scene, FindFirstObjectByType<PlayerController>().gameObject.transform.localPosition, "[???]", flags);
        }
        save.UpdatePersistentFlags(persFlags);
        save.UpdateDeathCount(deaths);
        checkpointSave = save;
        string path = "SAVE.sav"; 
        using (FileStream stream = File.Open(Path.Combine(Application.persistentDataPath, path), FileMode.OpenOrCreate))
        {
            SAVEFileIO.WriteFile(ref save, stream);
        }
    }
    public void DeactivateCheckpoint()
    {
        checkpointEnabled = false;
        checkpointPos = Vector3.zero;
    }
    public void DisablePlayerMovement(bool deactivatePartyMembers)
    {
        if (FindFirstObjectByType<PlayerController>() != null)
        {
            FindFirstObjectByType<PlayerController>().SetMovement(newMove: false);
        }
        if (deactivatePartyMembers)
        {
            PlayerController[] array = FindObjectsOfType<PlayerController>();
            for (int i = 0; i < array.Length; i++)
            {
                array[i].Deactivate();
            }
        }
        menuIsOpen = true;
    }
    
    public void EnablePlayerMovement()
    {
        if (FindFirstObjectByType<PlayerController>() != null)
        {
            FindFirstObjectByType<PlayerController>().SetMovement(true);
            FindFirstObjectByType<PlayerController>().Activate();
        }
        menuDisabled = false;
        menuLocked = false;
        menuIsOpen = false;
    }
    public void StartBattle(int newBattleId, LoadSceneMode sceneMode = LoadSceneMode.Additive)
    {
        battleId = newBattleId;
        SceneManager.LoadScene(10, sceneMode);
        SceneManager.sceneLoaded += OnBattleLoaded;
    }
    public void OnBattleLoaded(Scene ascene, LoadSceneMode aMode)
    {
        SceneManager.sceneLoaded -= OnBattleLoaded;
        SceneManager.SetActiveScene(ascene);
        GameObject obj = GameObject.Find("BattleFadeObj");
        GameObject obj2 = new GameObject("SOUL");
        obj2.AddComponent<SOUL>();
        obj2.GetComponent<SOUL>().CreateSOUL(new Color(1f, 0f, 0f), monster: false, player: true);
        obj2.GetComponent<SpriteRenderer>().sortingOrder = 500;
        obj.GetComponent<Fade>().FadeIn(5);
        Instantiate(Resources.Load<GameObject>("battle/BattleManager")).GetComponent<BattleManager>().StartBattle(battleId);
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
    }
    
    public void EndBattle(int battleEndState, bool force = false)
    {
        this.battleEndState = battleEndState;
        if (battleId == 75)
        {
            PlayMusic("zoneMusic");
        }
        
        SceneManager.UnloadSceneAsync("Battle");
        SceneManager.sceneUnloaded += OnBattleUnloaded;
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 1;
    }
    
    public void OnBattleUnloaded(Scene ascene)
    {
        SceneManager.sceneUnloaded -= OnBattleUnloaded;
        SpriteRenderer[] componentsInChildren = GameObject.Find("MAP").GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < componentsInChildren.Length; i++)
        {
            componentsInChildren[i].enabled = true;
        }
        Collider2D[] componentsInChildren2 = GameObject.Find("MAP").GetComponentsInChildren<Collider2D>();
        for (int i = 0; i < componentsInChildren2.Length; i++)
        {
            componentsInChildren2[i].enabled = true;
        }
        AudioSource[] componentsInChildren3 = GameObject.Find("MAP").GetComponentsInChildren<AudioSource>();
        for (int i = 0; i < componentsInChildren3.Length; i++)
        {
            componentsInChildren3[i].enabled = true;
        }
        TilemapRenderer[] componentsInChildren4 = GameObject.Find("MAP").GetComponentsInChildren<TilemapRenderer>();
        foreach (TilemapRenderer tilemapRenderer in componentsInChildren4)
        {
            if (tilemapRenderer.GetComponent<Tilemap>().enabled)
            {
                tilemapRenderer.enabled = true;
            }
        }
        SpriteMask[] componentsInChildren5 = GameObject.Find("MAP").GetComponentsInChildren<SpriteMask>();
        for (int i = 0; i < componentsInChildren5.Length; i++)
        {
            componentsInChildren5[i].enabled = true;
        }
        FindFirstObjectByType<PlayerController>().GetComponent<SpriteRenderer>().enabled = true;
        FindFirstObjectByType<PlayerController>().SetCollision(onoff: true);
        PlayerController[] array = UnityEngine.Object.FindObjectsOfType<PlayerController>();
        for (int i = 0; i < array.Length; i++)
        {
            array[i].GetComponent<SpriteRenderer>().enabled = true;
        }
        //ForceTogglePlayers(tog: true);
        EnablePlayerMovement();
        ResumeMusic(12);
        if ((bool)FindFirstObjectByType<LostCoreMusic>())
        {
            FindFirstObjectByType<LostCoreMusic>().SetDanger(danger: false);
        }
        FindFirstObjectByType<Fade>().FadeIn(12);
        /*
        if (!forcedBattleEnd)
        {
            EndBattleHandler.DoEndBattle(battleId, battleEndState);
        }
        else
        {
            forcedBattleEnd = false;
        }*/
        battleId = 0;
        battleEndState = -1;
    }
    public int HandleDamageCalculations(int hp, float damageMulti, bool applyDamageImmediately = true)
    {
        SOUL sOUL = FindFirstObjectByType<SOUL>();
        int a = hp;
        float num = hp;
            float num3 = num;
            float num4 = GetDEF();
            float num6 = 1f + (float)(GetLV() / 2) / 10f;
            num3 *= num6;
            if ((bool)sOUL && sOUL.IsShieldActive())
            {
                num3 *= 2f / 3f;
            }
            num3 *= damageMulti;
            if (num3 < 1f)
            {
                num3 = 1f;
            }
            {
                int num8 = Mathf.RoundToInt(num3);
                if (applyDamageImmediately)
                {
                    Damage(num8);
                }
                a -= num8;
            }
        //PartyPanels partyPanels = FindFirstObjectByType<PartyPanels>();
        //partyPanels.UpdateHP(hp-a);
        return a;
    }
    public void Damage(int dmg)
    {
        int num = hp;
        int HPmod=0;
        if (hp <= 20)
            HPmod = 0;
        if (20 < hp && hp< 30)
            HPmod = 1;
        if (30 <= hp && hp < 40)
            HPmod = 2;
        if (40 <= hp && hp < 50)
            HPmod = 3;
        if (50 <= hp && hp < 60)
            HPmod = 4;
        if (60 <= hp && hp < 70)
            HPmod = 5;
        if (70 <= hp && hp < 80)
            HPmod = 6;
        if (80 <= hp && hp < 90)
            HPmod = 7;
        if (90 <= hp)
            HPmod = 8;

        int damage = Mathf.RoundToInt(dmg + HPmod - (GetDEF()/ 5));
        if (damage == 0)
            damage = 1;
        hp -= damage;
        PartyPanels partyPanels = FindFirstObjectByType<PartyPanels>();
        partyPanels.UpdateHP(hp);
        if (hp <= 0)
        {
            hp = 0;
        }
        if (hp == 0)
        {
            Death();
        }
    }
    public List<int> GetItemList()
    {
        return items;
    }
    public void Death(int specialText = -1)
    {
        deaths++;
        SetSessionFlag(7, specialText);
        if (FileExists())
        {
            SaveFile(savepoint: false);
        }
        //inSingleBattle = false;
        SceneManager.LoadScene(16, LoadSceneMode.Single);
        spawnPos = Vector2.zero;
        if (FindFirstObjectByType<SOUL>() != null)
        {
            spawnPos = FindFirstObjectByType<SOUL>().transform.position - GameObject.Find("BattleCamera").transform.position;
        }
        else if (FindFirstObjectByType<ActionSOUL>() != null)
        {
            if (FindFirstObjectByType<ActionSOUL>().transform.childCount > 0)
            {
                spawnPos = FindFirstObjectByType<ActionSOUL>().transform.GetChild(0).position - FindFirstObjectByType<CameraController>().transform.position;
            }
            else
            {
                spawnPos = FindFirstObjectByType<ActionSOUL>().transform.position - FindFirstObjectByType<CameraController>().transform.position;
            }
        }
        else if (FindFirstObjectByType<PlayerController>() != null)
        {
            spawnPos = FindFirstObjectByType<PlayerController>().transform.position - FindFirstObjectByType<CameraController>().transform.position;
        }
        SceneManager.sceneLoaded += OnDeathScreenLoaded;
    }

    public void OnDeathScreenLoaded(Scene ascene, LoadSceneMode aMode)
    {
        DisablePlayerMovement(deactivatePartyMembers: true);
        aud.Stop();
        mp.Stop();
        SceneManager.sceneLoaded -= OnDeathScreenLoaded;
    }
    public int GetNumDeaths()
    {
        return deaths;
    }
    public Vector2 GetSpawnPos()
    {
        return spawnPos;
    }
}
