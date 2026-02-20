using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : TranslatableSelectableBehaviour
{
    protected GameManager gm;

    protected BattleCamera cam;

    protected MusicPlayer mus;

    protected AudioSource aud;

    protected AudioSource aud2;

    protected GameObject soul;

    protected GameObject target;

    protected BulletBoard bb;

    protected Fade fadeObj;

    protected bool doneIntroFade;

    protected BattleBG bg;

    //protected ShakingText st;

    protected GameObject selObj;

    protected GameObject selObj2;

    protected bool doPage2;

    protected int selTarget;

    protected int actChoice;

    protected int battleId;

    protected bool startedBattle;

    protected EnemyBase[] enemies;

    protected bool isBoss;

    protected int curHP;

    protected TextUT boxText;

    protected RectTransform boxPortrait;

    protected string curFlavor;

    protected bool flavorPlayedOnce;

    protected string[] diag;

    protected int curDiag;

    protected int finalDiag;

    protected int state;

    protected AttackBase curAtk;

    protected PartyPanels partyPanels;

    protected int partySize;

    protected int partyTurn;

    protected int[] playerSelection = new int[3] { 0, 0, 0 };

    protected int deviousChance = 10;

    protected int[] revivalTurns = new int[3];

    protected bool[] defending = new bool[3];

    protected bool selectingMagic;

    protected bool actMagicSelect;

    protected bool castingRedBuster;

    protected bool castingDualHeal;

    protected int firstAvail;

    protected int niceActIndex;

    protected bool sparingThisRound;

    protected bool fightingThisRound;

    protected bool firstButton;

    protected int buttonIndex;

    protected bool axisIsDown;

    protected bool isSOULOut;

    protected int endState;

    protected int curDT;

    protected int frames;

    protected int maxFrames;

    protected bool didSoulSparkle;

    protected bool FightAfterZ;
    protected bool dialogueOver;

    protected int ItemIndex = -1;
    
    protected DescriptionBox descriptionBox;

    public override Dictionary<string, string[]> GetDefaultStrings()
    {
        Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>();
        dictionary.Add("act_check", new string[1] { "Check" });
        dictionary.Add("error_acts", new string[2] { "* But it didn't exist.\n* So this is an error.", "* But nothing happened.\n* So this is an error." });
        dictionary.Add("check_desc_base", new string[1] { "* {0} - ATK {1} DEF {2}\n{3}" });
        return dictionary;
    }

    protected virtual void Awake()
    {
        SetStrings(GetDefaultStrings(), GetType());
        endState = 0;
        startedBattle = false;
        firstButton = true;
    }

    protected virtual void Start()
    {
    }

    protected void Initialize()
    {
        Destroy(GameObject.Find("OWSoul(Clone)"));
        gm = FindFirstObjectByType<GameManager>();
        cam = FindFirstObjectByType<BattleCamera>();
        mus = GetComponent<MusicPlayer>();
        aud = base.gameObject.AddComponent<AudioSource>();
        aud2 = base.gameObject.AddComponent<AudioSource>();
        bb = FindFirstObjectByType<BulletBoard>();
        fadeObj = GameObject.Find("BattleFadeObj").GetComponentInChildren<Fade>();
        bg = FindFirstObjectByType<BattleBG>();
        boxText = base.gameObject.AddComponent<TextUT>();
        boxText.SetParent(GameObject.Find("BattleCanvas").transform);
        soul = GameObject.Find("SOUL");
        soul.GetComponent<SOUL>().AdjustSOULColor();
        partyPanels = FindFirstObjectByType<PartyPanels>();
        ChangeHP();
        partyTurn = 0;
        state = 0;
        actChoice = 0;
        selTarget = 0;
        buttonIndex = 0;
        SelectButton(buttonIndex);
        axisIsDown = false;
        descriptionBox = FindFirstObjectByType<DescriptionBox>();
        didSoulSparkle = false;
        isSOULOut = false;
    }
    
    public virtual void StartBattle(int id)
    {
        battleId = id;
        Initialize();
        enemies = EnemyGenerator.GetEnemies(battleId);
        object[] music = EnemyGenerator.GetMusic(battleId);
        PlayMusic(music[0].ToString(), float.Parse(music[1].ToString()));
        object[] battleBG = EnemyGenerator.GetBattleBG(battleId);
        bg.StartBG(int.Parse(battleBG[0].ToString()), float.Parse(battleBG[1].ToString()), float.Parse(battleBG[2].ToString()), (Color)battleBG[3], (bool)battleBG[4]);
        curFlavor = EnemyGenerator.GetApproachText(battleId);
        isBoss = battleId == 14 || battleId == 29 || battleId == 40 || battleId == 52 || battleId == 53 || battleId == 54 || battleId == 73;
        if (state == 5)
        {
            SendBattleEvents(4);
        }
        startedBattle = true;
    }
    
    protected virtual void Update()
    {
        if (!startedBattle)
        {
            return;
        }
        if (!fadeObj.IsPlaying() && !doneIntroFade)
        {
            soul.GetComponent<SpriteRenderer>().sortingOrder = 199;
            doneIntroFade = true;
        }
        int num = gm.GetHP();
        float num2 = gm.GetMaxHP();
            //st.StartShake((int)((float)num / num2 * (float)num4));
        if (state == 0)
        {
            if (!boxText.Exists())
            {
                StartText(curFlavor, new Vector2(-4f, -95f), "snd_txtbtl");
            }
            if ((UTInput.GetButton("X") || UTInput.GetButton("C") || flavorPlayedOnce) && boxText.IsPlaying())
            {
                boxText.SkipText(sound: false);
                flavorPlayedOnce = true;
            }
            soul.GetComponent<SOUL>().SetFrozen(boo: true);
            soul.GetComponent<SpriteRenderer>().enabled = true;
            if (partyTurn == 0 && GameObject.Find("ACT").GetComponent<BattleButton>().GetButtonType() != "act")
            {
                GameObject.Find("ACT").GetComponent<BattleButton>().ChangeButtonType("act");
            }
            else if ((partyTurn == 1 || partyTurn == 2) && GameObject.Find("ACT").GetComponent<BattleButton>().GetButtonType() != "magic")
            {
                GameObject.Find("ACT").GetComponent<BattleButton>().ChangeButtonType("magic");
            }
            if (Mathf.RoundToInt(UTInput.GetAxisDown("Horizontal")) != 0 && !axisIsDown)
            {
                buttonIndex += Mathf.RoundToInt(UTInput.GetAxisRaw("Horizontal"));
                if (buttonIndex > 3)
                {
                    buttonIndex = 0;
                }
                else if (buttonIndex < 0)
                {
                    buttonIndex = 3;
                }
                axisIsDown = true;
                buttonIndex = Mathf.Abs(buttonIndex % 4);
                SelectButton(buttonIndex);
            }
            else if (Mathf.RoundToInt(UTInput.GetAxisDown("Horizontal")) == 0 && axisIsDown)
            {
                axisIsDown = false;
            }
            if (UTInput.GetButtonDown("Z"))
            {
                bool flag = true;
                string[,] array = new string[4, 2];
                string[,] array2 = new string[3, 2];
                int i = 0;
                int num5 = 0;
                bool flag2 = false;
                bool flag3 = false;
                selObj = new GameObject("SelectTier1");
                selObj.layer = 5;
                selObj.AddComponent<RectTransform>();
                selObj.transform.SetParent(GameObject.Find("BattleCanvas").transform);
                selObj2 = new GameObject("SelectTier2");
                selObj2.layer = 5;
                selObj2.AddComponent<RectTransform>();
                selObj2.transform.SetParent(GameObject.Find("BattleCanvas").transform);
                firstAvail = -1;
                if (buttonIndex == 0)
                {
                    array = GetEnemyListArray();
                    DrawEnemyBars(selObj);
                    flag3 = true;
                    flag = false;
                    playerSelection[1] = 0;
                }
                else if (buttonIndex == 1)
                {
                    selectingMagic = true;
                    int num6 = 0;
                    int num7 = 0;
                    for (int j = 0; j < enemies.Length; j++)
                    {
                        if (!enemies[j].IsDone())
                        {
                            num6++;
                            num7 = j;
                        }
                    }
                    array[0, 0] = "* "+enemies[0].GetName();
                    playerSelection[1] = 1;
                    flag = false;
                }
                else if (buttonIndex == 2)
                {
                    GameObject obj = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("ui/TextBase"), selObj.transform);
                    obj.name = "PAGE1";
                    obj.transform.localPosition = new Vector2(330f, -170f);
                    obj.transform.localScale = new Vector3(1f, 1f, 1f);
                    obj.GetComponent<Text>().text = "PAGE 1";
                    GameObject obj2 = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("ui/TextBase"), selObj2.transform);
                    obj2.name = "PAGE2";
                    obj2.transform.localPosition = new Vector2(330f, -170f);
                    obj2.transform.localScale = new Vector3(1f, 1f, 1f);
                    obj2.GetComponent<Text>().text = "PAGE 2";
                    List<int> itemListPerTurn = GetItemListPerTurn();
                    doPage2 = false;
                    int counter = 0;
                    
                    foreach (int item in itemListPerTurn)
                    {
                        if (item == -1)
                        {
                            continue;
                        }
                        counter++;
                        flag = false;
                        if (flag2)
                        {
                            doPage2 = true;
                            array2[i, num5] = "* " + Items.ShortItemName(item, isBoss);
                        }
                        else
                        {
                            array[i, num5] = "* " + Items.ShortItemName(item, isBoss);
                        }
                        num5++;
                        if (num5 == 2)
                        {
                            num5 = 0;
                            i++;
                            if (i == 2)
                            {
                                i = 0;
                                flag2 = true;
                            }
                        }
                    }
                    if (counter < 5)
                    {
                        obj.GetComponent<Text>().text = " ".ToString();
                        Debug.Log("idk");
                    }
                    playerSelection[1] = 2;
                }
                else if (buttonIndex == 3)
                {
                    array[0, 0] = "* Spare";
                    bool flag4 = false;
                    for (int k = 0; k < enemies.Length; k++)
                    {
                        if (enemies[k].CanSpare() && !enemies[k].IsDone())
                        {
                            flag4 = true;
                        }
                    }
                    if (flag4)
                    {
                        array[0, 0] = "<color=#ffff00ff>* Spare</color>";
                    }
                    //array[1, 0] = "* Flee";
                    flag = false;
                    playerSelection[1] = 3;

                }

                for (num5 = 0; num5 <= 1; num5++)
                {
                    for (i = 0; i <= 1; i++)
                    {
                        if (array[i, num5] == null)
                        {
                            array[i, num5] = "";
                        }
                    }
                }
                boxText.SkipText(sound: false);
                if (!flag)
                {
                    flavorPlayedOnce = true;
                    if (firstAvail == -1)
                    {
                        firstAvail = 0;
                    }
                    selObj.AddComponent<Selection>().CreateSelections(array, new Vector2(-220f, -138f), new Vector2(240f, -32f), new Vector2(-28f, 95f), "DTM-Mono", useSoul: true, makeSound: true, this, 0);
                    selObj.transform.localScale = new Vector2(1f, 1f);
                    selObj.GetComponent<Selection>().SetSelection(new Vector2(firstAvail, 0f), playSound: false);
                    selObj2.AddComponent<Selection>().CreateSelections(array2, new Vector2(-220f, -138f), new Vector2(240f, -32f), new Vector2(-28f, 95f), "DTM-Mono", useSoul: true, makeSound: true, this, 1);
                    selObj2.transform.localScale = new Vector2(1f, 1f);
                    selObj2.GetComponent<Selection>().Disable();
                    selObj2.SetActive(value: false);
                    if (flag3)
                    {
                        HandleEnemyNameColor();
                    }
                    ResetText();
                    state = 1;
                }
                else
                {
                    UnityEngine.Object.Destroy(selObj);
                }
                aud.clip = Resources.Load<AudioClip>("sounds/snd_select");
                aud.Play();
            }
        }
        if (state == 1)
        {
            if (buttonIndex == 2 && UTInput.GetAxisRaw("Horizontal") == 1f && selObj.GetComponent<Selection>().GetIndex()[1] == 1f && doPage2 && gm.GetItem(4 + 2 * (int)selObj.GetComponent<Selection>().GetIndex()[0]) != -1 && !selObj.GetComponent<Selection>().AxisDown())
            {
                Vector2 index = selObj.GetComponent<Selection>().GetIndex();
                if (GetItemListPerTurn().Count - 4 > 2)
                {
                    index -= new Vector2(0f, 1f);
                }
                else
                {
                    index -= new Vector2((index.x == 1f) ? 1 : 0, 1f);
                }
                selObj.GetComponent<Selection>().Disable();
                selObj.SetActive(value: false);
                selObj2.SetActive(value: true);
                selObj2.GetComponent<Selection>().Enable();
                selObj2.GetComponent<Selection>().SetSelection(index);
                selObj2.GetComponent<Selection>().SetAxisDown(boo: true);
                gm.PlayGlobalSFX("sounds/snd_menumove");
                state = 2;
            }
            if (UTInput.GetButtonDown("X"))
            {
                UnityEngine.Object.Destroy(selObj);
                UnityEngine.Object.Destroy(selObj2);
                state = 0;
                SelectButton(buttonIndex);
            }
        }
        if (state == 2)
        {
            if (buttonIndex == 2 && UTInput.GetAxisRaw("Horizontal") == -1f && selObj2.GetComponent<Selection>().GetIndex()[1] == 0f && !selObj2.GetComponent<Selection>().AxisDown())
            {
                Vector2 selection = selObj2.GetComponent<Selection>().GetIndex() + new Vector2(0f, 1f);
                selObj2.GetComponent<Selection>().Disable();
                selObj2.SetActive(value: false);
                selObj.SetActive(value: true);
                selObj.GetComponent<Selection>().Enable();
                selObj.GetComponent<Selection>().SetSelection(selection);
                selObj.GetComponent<Selection>().SetAxisDown(boo: true);
                gm.PlayGlobalSFX("sounds/snd_menumove");
                state = 1;
            }
            if (UTInput.GetButtonDown("X"))
            {
                if (buttonIndex == 1)
                {
                    for (int l = 0; l < 3; l++)
                    {
                        if ((bool)GameObject.Find("PartyMemberHP" + l))
                        {
                            UnityEngine.Object.Destroy(GameObject.Find("PartyMemberHP" + l));
                        }
                    }
                    if (partyTurn == 0)
                    {
                        //descriptionBox.Hide();
                        //UnityEngine.Object.FindObjectOfType<TPBar>().UpdateTPPreviewBar(0);
                    }
                }
                if (buttonIndex == 2)
                {
                    UnityEngine.Object.Destroy(selObj);
                    UnityEngine.Object.Destroy(selObj2);
                    state = 0;
                    SelectButton(buttonIndex);
                    //descriptionBox.Hide();
                }
                else
                {
                    selObj2.SetActive(value: false);
                    selObj.SetActive(value: true);
                    state = 1;
                }
            }
        }
        if (state == 3)
        {
            if (!boxText.IsPlaying() && (bool)FindFirstObjectByType<SpecialACT>() && !FindFirstObjectByType<SpecialACT>().IsActivated())
            {
                FindFirstObjectByType<SpecialACT>().Activate();
            }
            if ((UTInput.GetButton("X") || UTInput.GetButton("C")) && boxText.IsPlaying())
            {
                boxText.SkipText();
                if ((bool)FindFirstObjectByType<SpecialACT>())
                {
                    FindFirstObjectByType<SpecialACT>().Activate();
                }
            }
            else if ((((UTInput.GetButtonDown("Z") || UTInput.GetButton("C")) && !boxText.IsPlaying()) || !boxText.GetGameObject()) && (!FindFirstObjectByType<SpecialACT>() || !FindFirstObjectByType<SpecialACT>().IsActivated()))
            {
                bool flag5 = false;
                if ((UTInput.GetButtonDown("Z") || UTInput.GetButton("C")) && (bool)boxText.GetGameObject())
                {
                    curDiag++;
                    flag5 = true;
                    if (!FindFirstObjectByType<SpecialACT>())
                    {
                        ResetText();
                    }
                }
                bool flag6 = true;
                EnemyBase[] array3 = enemies;
                for (int m = 0; m < array3.Length; m++)
                {
                    if (array3[m].IsShaking())
                    {
                        flag6 = false;
                    }
                }
                if ((!boxText.Exists() || flag5) && !FindFirstObjectByType<SpecialAttackEffect>() && flag6)
                {
                    if (curDiag > finalDiag)
                    {
                        if (boxText.Exists())
                        {
                            ResetText();
                        }
                        if (!FindFirstObjectByType<SpecialACT>())
                        {
                            if (niceActIndex < 3 || (niceActIndex == 3 && (fightingThisRound || sparingThisRound)))
                            {
                                //AdvanceToEnemyTurn();
                                AdvancePlayerTurn();
                            }
                            else
                            {
                                AdvanceToEnemyTurn();
                            }
                        }
                    }
                    else
                    {
                        StartText(diag[curDiag], new Vector2(-4f, -134f), "snd_txtbtl");
                    }
                }
            }
        }
        if(state == 9)
        {
            soul.GetComponent<SpriteRenderer>().enabled = true;
            AdvanceToEnemyTurn();
        }
        if (state == 8)
        {
            state = 9;
        }
        if (state == 7 && ((target==null || target.GetComponentInChildren<FightTarget>()==null) || (!target.GetComponentInChildren<FightTarget>().IsGoing() && !FindFirstObjectByType<SpecialAttackEffect>())))
        {
            state = 8;
        }
        if (state == 4)
        {
            bool flag7 = false;
            EnemyBase[] array3 = enemies;
            for (int m = 0; m < array3.Length; m++)
            {
                if (array3[m].IsTalking())
                {
                    flag7 = true;
                }
            }
            if (!bb.IsPlaying() && !flag7)
            {
                soul.GetComponent<SOUL>().SetFrozen(boo: false);
                state = 5;
            }
        }
        if (state == 5 && !bb.IsPlaying())
        {
            if (curAtk == null)
            {
                soul.GetComponent<SOUL>().SetControllable(boo: false);
                soul.GetComponent<SpriteRenderer>().enabled = false;
                bb.ResetSize();
                state = 6;
                SendBattleEvents();
            }
            else if (!curAtk.HasStarted())
            {
                curAtk.StartAttack();
            }
        }
        if (state == 6 && !bb.IsPlaying())
        {
            for (int n = 0; n < 1; n++)
            {
                if (gm.GetHP() <= 0)
                {
                    revivalTurns[n]--;
                    if (revivalTurns[n] == 0)
                    {
                        gm.SetHP(gm.GetMaxHP(n) / 4);
                    }
                }
                else
                {
                    revivalTurns[n] = 0;
                }
            }
            ChangeHP();
            flavorPlayedOnce = false;
            if (AllEnemiesDone())
            {
                bb.SetBGOrder(100);
                EndNormalFight(customMessage: false, "");
            }
            else
            {
                ChangeFlavorText();
                bb.SetBGOrder(100);
                partyTurn = 0;
                state = 0;
                SelectButton(buttonIndex);
                soul.GetComponent<SOUL>().SetGravityDirection(Vector2.down);
            }
        }
        if (state == 10)
        {
            if ((UTInput.GetButton("X") || UTInput.GetButton("C")) && boxText.IsPlaying())
            {
                boxText.SkipText();
            }
            else if ((UTInput.GetButtonDown("Z") || UTInput.GetButton("C")) && !boxText.IsPlaying())
            {
                gm.EndBattle(endState);
            }
        }
        if (state == 11)
        {
            fadeObj.FadeOut(11);
            state = 12;
        }
        if (state == 12 && !fadeObj.IsPlaying())
        {
            gm.EndBattle(endState);
        }
    }
    
    protected void SelectButton(int buttonIndex)
    {
        string[] array = new string[4] { "FIGHT", "ACT", "ITEM", "MERCY" };
        for (int i = 0; i < 4; i++)
        {
            BattleButton component = GameObject.Find(array[i]).GetComponent<BattleButton>();
            if (buttonIndex == i)
            {
                soul.transform.SetParent(component.transform);
                soul.transform.localPosition = new Vector2(-0.82f, -0.022f);
                soul.transform.SetParent(null);
                component.Select(boo: true);
            }
            else
            {
                component.Select(boo: false);
            }
        }
    }

    protected virtual void LateUpdate()
    {
        if (!startedBattle)
        {
            return;
        }
        
        if ((state == 1 || state == 2) && buttonIndex == 2 && (bool)selObj.transform.Find("PAGE1"))
        {
            int num = -1;
            if (state == 1)
            {
                num = (int)selObj.GetComponent<Selection>().GetIndex()[1] + (int)selObj.GetComponent<Selection>().GetIndex()[0] * 2;
            }
            else if (state == 2)
            {
                num = (int)selObj2.GetComponent<Selection>().GetIndex()[1] + (int)selObj2.GetComponent<Selection>().GetIndex()[0] * 2 + 4;
            }
            ItemIndex = num;
        }
        else
        {
            ItemIndex = -1;
        }
            Vector3 vector = new Vector3(69f, 420f);
        if ((bool)selObj && (bool)selObj.GetComponent<Selection>() && selObj.GetComponent<Selection>().IsEnabled() && selObj.activeInHierarchy)
        {
            vector = selObj.GetComponent<Selection>().GetSOUL().transform.localPosition / 48f;
        }
        if ((bool)selObj2 && (bool)selObj2.GetComponent<Selection>() && selObj2.GetComponent<Selection>().IsEnabled() && selObj2.activeInHierarchy)
        {
            vector = selObj2.GetComponent<Selection>().GetSOUL().transform.localPosition / 48f;
        }
        if (vector != new Vector3(69f, 420f))
        {
            soul.transform.position = vector;
        }
        if (doneIntroFade)
        {
            if (state == 1 || state == 2)
            {
                soul.GetComponent<SpriteRenderer>().sortingOrder = 401;
            }
            else if (state == 3 || state == 0)
            {
                soul.GetComponent<SpriteRenderer>().sortingOrder = 199;
            }
        }
        bool t = FindFirstObjectByType<TextUT>().IsPlaying();
        if(FightAfterZ && !t)
        {
            if (UTInput.GetButtonUp("Z")){
                FightAfterZ = false;
                AdvanceToEnemyTurn();
            }
        }
    }
    
    public override void MakeDecision(Vector2 index, int id)
    {
        actChoice = 0;
        if (buttonIndex == 0)
        {
            selTarget = (int)index[0];
            UnityEngine.Object.Destroy(selObj);
            UnityEngine.Object.Destroy(selObj2);
            aud.clip = Resources.Load<AudioClip>("sounds/snd_select");
            aud.Play();
            DecideMemberAction(selTarget, 0, 0);
        }
        if (buttonIndex == 1)
        {
            if (id == 0)
            {
                firstAvail = -1;
                selObj.GetComponent<Selection>().Reset();
                actMagicSelect = false;
                bool flag2 = false;
                int childCount = selObj.transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    UnityEngine.Object.DestroyImmediate(selObj.transform.GetChild(0).gameObject);
                }
                string[,] array;
                if (index == Vector2.zero)
                {
                    array = GetEnemyListArray();
                    DrawEnemyBars(selObj);
                    flag2 = true;
                }
                if (firstAvail == -1)
                {
                    firstAvail = 0;
                }
                array = new string[1, 1];
                array[0, 0] = "* Check";
                selObj.GetComponent<Selection>().CreateSelections(array, new Vector2(-220f, -138f), new Vector2(240f, -32f), new Vector2(-28f, 95f), "DTM-Mono", useSoul: true, makeSound: true, this, 0);
                selObj.transform.localScale = new Vector2(1f, 1f);
                selObj.GetComponent<Selection>().SetSelection(new Vector2(firstAvail, 0f), playSound: false);
                if (flag2)
                {
                    HandleEnemyNameColor();
                }
                aud.clip = Resources.Load<AudioClip>("sounds/snd_select");
                aud.Play();
                playerSelection[0] = id;
                playerSelection[1] = 1;
                DecideMemberAction(0, 1, 0);

            }
            else
            {
                switch (id)
                {
                    case 1:
                        {
                            int num5 = (int)index[0] * 2 + (int)index[1];
                            if (partyTurn == 0)
                            {
                                string text2 = enemies[selTarget].GetActNames()[num5];
                                bool num6 = !text2.StartsWith("S!") && !text2.StartsWith("N!") && !text2.StartsWith("SN!") && !text2.StartsWith("KS!");
                                {
                                    selObj2.GetComponent<Selection>().GetSelectionTexts()[(int)index[0], (int)index[1]].GetComponent<AudioSource>().Stop();
                                    aud.clip = Resources.Load<AudioClip>("sounds/snd_cantselect");
                                    aud.Play();
                                }
                            }
                            else
                            {
                                UnityEngine.Object.Destroy(selObj);
                                UnityEngine.Object.Destroy(selObj2);
                                if (partyTurn > 0)
                                {
                                    DecideMemberAction(num5 / 2, 1, selTarget);
                                }
                                else
                                {
                                    DecideMemberAction(num5 / 2, 6, selTarget);
                                }
                                aud.clip = Resources.Load<AudioClip>("sounds/snd_select");
                                aud.Play();
                            }
                            break;
                        }
                    case 2:
                        {
                            int num4 = (int)index[0] * 2 + (int)index[1];
                            //DebugTools.UseTool(DebugTools.GetKeys()[num4]);
                            aud.clip = Resources.Load<AudioClip>("sounds/snd_select");
                            aud.Play();
                            break;
                        }

                }
                playerSelection[0] = id;
                playerSelection[1] = 1;
            }
        }
        if (buttonIndex == 2)
        {
            if (id != 2)
            {
                int num7 = (int)index[0] * 2 + (int)index[1];
                selObj.SetActive(value: true);
                selObj2.SetActive(value: false);
                state = 1;
                int num8 = num7 + 4 * id;
                playerSelection[0] = id;
                playerSelection[2] = num7;
                playerSelection[1] = 2;
                UnityEngine.Object.Destroy(selObj.transform.Find("PAGE1").gameObject);
                selObj.GetComponent<Selection>().Reset();
                //DecideMemberAction(selTarget, 0, 0);

            }
            if (id == 2)
            {
                int num7 = 4+(int)index[0] * 2 + (int)index[1];
                UnityEngine.Object.Destroy(selObj);
                UnityEngine.Object.Destroy(selObj2);
                playerSelection[0] = id;
                playerSelection[2] = num7;
                playerSelection[1] = 2;
                //DecideMemberAction(partySelections[partyTurn][0], 2, partySelections[partyTurn][2]);
            }
            aud.clip = Resources.Load<AudioClip>("sounds/snd_select");
            aud.Play();
            DecideMemberAction(selTarget, 0, 0);
        }
        if (buttonIndex == 3)
        {
            UnityEngine.Object.Destroy(selObj);
            UnityEngine.Object.Destroy(selObj2);
            if (index[0] == 1f)
            {
                //UnityEngine.Object.FindObjectOfType<TPBar>().SetDefendingMember(partyTurn, tpToGain: true);
                //partyPanels.SetAsDefending(partyTurn, defend: true);
                defending[partyTurn] = true;
            }
            if (index[0] == 2f)
            {
                gm.EndBattle(0, force: true);
                gm.EnablePlayerMovement();
            }
            DecideMemberAction(0, 3, (int)index[0]);
            aud.clip = Resources.Load<AudioClip>("sounds/snd_select");
            aud.Play();
        }
    }

    protected virtual void DrawEnemyBars(GameObject selObj)
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].IsDone())
            {
                continue;
            }
            //Instantiate(Resources.Load<GameObject>("battle/HPMercyLabel"), selObj.transform);
            GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("battle/enemies/FightEnemyHP"), selObj.transform);
            gameObject.name = "CoolGamerHP" + i;
            gameObject.transform.localPosition += new Vector3(220f, -32 * i - 36);
            int num = Mathf.CeilToInt((float)enemies[i].GetHP() / (float)enemies[i].GetMaxHP() * 100f);
            if (num > 100)
            {
                num = 100;
            }
            else if (num < 1)
            {
                num = 1;
            }
            float f = (float)num * 0.75f;
            //gameObject.transform.Find("fg").GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.CeilToInt(f), 17f);
            //gameObject.transform.Find("Text").GetComponent<Text>().text = num + "%";
            //gameObject.transform.Find("TextShadow").GetComponent<Text>().text = num + "%";
            GameObject gameObject2 = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("battle/enemies/FightEnemyHP"), selObj.transform);
            gameObject2.name = "CoolGamerMercy" + i;
            gameObject2.transform.localPosition += new Vector3(310f, -32 * i - 36);
            if (enemies[i].RenderSpareBar())
            {
                int num2 = enemies[i].GetSatisfactionLevel();
                if (num2 > 100)
                {
                    num2 = 100;
                }
                else if (num2 < 0)
                {
                    num2 = 0;
                }
                float f2 = (float)num2 * 0.75f;
                //gameObject2.transform.Find("fg").GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Ceil(f2), 17f);
                //gameObject2.transform.Find("fg").GetComponent<Image>().color = new Color(1f, 1f, 0f);
                //gameObject2.transform.Find("bg").GetComponent<Image>().color = new Color32(byte.MaxValue, 94, 27, byte.MaxValue);
                gameObject2.transform.Find("Text").GetComponent<Text>().text = num2 + "%";
                gameObject2.transform.Find("Text").GetComponent<Text>().color = new Color32(142, 12, 0, byte.MaxValue);
                gameObject2.transform.Find("TextShadow").GetComponent<Text>().text = num2 + "%";
            }
            else
            {
                gameObject2.transform.Find("nomercy").GetComponent<Image>().enabled = true;
                //gameObject2.transform.Find("fg").GetComponent<Image>().color = new Color32(byte.MaxValue, 94, 27, byte.MaxValue);
                //gameObject2.transform.Find("bg").GetComponent<Image>().color = new Color32(byte.MaxValue, 94, 27, byte.MaxValue);
                gameObject2.transform.Find("Text").GetComponent<Text>().enabled = false;
                gameObject2.transform.Find("TextShadow").GetComponent<Text>().enabled = false;
            }
            if ((int)FindFirstObjectByType<GameManager>().GetFlag(94) == 1)
            {
                Image[] componentsInChildren = gameObject.transform.Find("corners").GetComponentsInChildren<Image>();
                for (int j = 0; j < componentsInChildren.Length; j++)
                {
                    componentsInChildren[j].enabled = true;
                }
                componentsInChildren = gameObject2.transform.Find("corners").GetComponentsInChildren<Image>();
                for (int j = 0; j < componentsInChildren.Length; j++)
                {
                    componentsInChildren[j].enabled = true;
                }
            }
            
        }
    }
    
    protected string[,] GetMemberListArray()
    {
        string[,] array = new string[3, 2]
        {
            { "* Pawn", null },
            { null, null },
            { null, null }
        };
        return array;
    }
    
    protected string[,] GetEnemyListArray()
    {
        string[,] array = new string[4, 2];
        for (int i = 0; i < enemies.Length; i++)
        {
            if (!enemies[i].IsDone())
            {
                if (firstAvail == -1)
                {
                    firstAvail = i;
                }
                array[i, 0] = "* " + enemies[i].GetName();
            }
        }
        return array;
    }
    
    protected void HandleEnemyNameColor()
    {
        Selection selection = (((bool)selObj && selObj.activeInHierarchy) ? selObj.GetComponent<Selection>() : selObj2.GetComponent<Selection>());
        Color color = new Color32(0, 162, 232, byte.MaxValue);
        Color color2 = new Color(1f, 1f, 0f);
        for (int i = 0; i < enemies.Length; i++)
        {
            if (!enemies[i].IsDone())
            {
                bool num = enemies[i].IsTired() && enemies[i].CanSpare();
                Text component = selection.GetSelectionTexts()[i, 0].GetComponent<Text>();
                if (num)
                {
                    UnityEngine.UI.Gradient gradient = component.gameObject.AddComponent<UnityEngine.UI.Gradient>();
                    gradient.GradientType = UnityEngine.UI.Gradient.Type.Horizontal;
                    gradient.EffectGradient = new UnityEngine.Gradient
                    {
                        colorKeys = new GradientColorKey[2]
                        {
                            new GradientColorKey(color2, 0.2f),
                            new GradientColorKey(color, 1f)
                        }
                    };
                }
                else if (enemies[i].CanSpare())
                {
                    selection.GetSelectionTexts()[i, 0].GetComponent<Text>().color = color2;
                }
                else if (enemies[i].IsTired())
                {
                    selection.GetSelectionTexts()[i, 0].GetComponent<Text>().color = color;
                }
                if (enemies[i].CanSpare())
                {
                    UnityEngine.Object.Instantiate(Resources.Load<GameObject>("battle/SpareIcon"), selection.transform).transform.localPosition = new Vector3(-192 + (enemies[i].GetName().Length + 2) * 16, -82 + -32 * i);
                }
                if (enemies[i].IsTired())
                {
                    UnityEngine.Object.Instantiate(Resources.Load<GameObject>("battle/TiredIcon"), selection.transform).transform.localPosition = new Vector3(-172 + (enemies[i].GetName().Length + 2) * 16, -82 + -32 * i);
                }
            }
        }
    }

    public virtual void SendBattleEvents(int? state = null)
    {
        if (!state.HasValue)
        {
            state = this.state;
        }
        EnemyBase[] array = enemies;
        foreach (EnemyBase enemyBase in array)
        {
            if (!enemyBase.IsDone())
            {
                switch (state)
                {
                    case 4:
                        enemyBase.EnemyTurnStart();
                        break;
                    case 6:
                        enemyBase.EnemyTurnEnd();
                        break;
                    default:
                        throw new InvalidOperationException("No event for state " + state);
                }
            }
        }
    }

    public void ChangeFlavorText()
    {
        int i;
        for (i = 0; i < enemies.Length && enemies[i].IsDone(); i++)
        {
        }
        curFlavor = enemies[i].GetRandomFlavorText();
    }
    public void ButtonSFX()
    {
        if (!firstButton)
        {
            aud.clip = Resources.Load<AudioClip>("sounds/snd_menumove");
            aud.Play();
        }
        firstButton = false;
    }

    public void StartSOULDecision()
    {
        mus.Stop();
        isSOULOut = true;
    }

    public int GetBattleID()
    {
        return battleId;
    }

    public EnemyBase[] GetEnemies()
    {
        return enemies;
    }
    
    public void PlayMusic(string music, float pitch)
    {
        if (music != "" && music.Replace("_intro", "") != mus.CurrentMusic())
        {
            bool flag = music.EndsWith("_intro");
            mus.ChangeMusic(flag ? music.Replace("_intro", "") : music, flag, playImmediately: true);
            mus.GetSource().pitch = pitch;
        }
        else if ((bool)FindFirstObjectByType<LostCoreMusic>())
        {
            FindFirstObjectByType<LostCoreMusic>().SetDanger(danger: true);
        }
    }

    public void PlayMusic(string music, float pitch, bool hasIntro)
    {
        if (music != "" && music != mus.CurrentMusic())
        {
            mus.ChangeMusic(music, hasIntro, playImmediately: true);
            mus.GetSource().pitch = pitch;
        }
        else if ((bool)FindFirstObjectByType<LostCoreMusic>())
        {
            FindFirstObjectByType<LostCoreMusic>().SetDanger(danger: true);
        }
    }

    public void StopMusic()
    {
        mus.Stop();
    }
    
    public void FadeEndBattle()
    {
        fadeObj.FadeOut(11);
        state = 12;
    }

    public void FadeEndBattle(int endState)
    {
        this.endState = endState;
        FadeEndBattle();
    }

    public Fade GetBattleFade()
    {
        return fadeObj;
    }
    
    public virtual void DecideMemberAction(int target, int action, int extraData)
    {
            flavorPlayedOnce = true;
            GameObject.Find("ACT").GetComponent<BattleButton>().ChangeButtonType("act");
            soul.transform.SetParent(null);
            soul.transform.position = new Vector2(-0.055f, -1.63f);
            firstButton = true;
            
            niceActIndex = 0;
            //state = -1;
            soul.GetComponent<SpriteRenderer>().enabled = false;
            soul.transform.position = new Vector3(500f, 500f);
            SelectButton(-1);
            fightingThisRound = false;
            AdvancePlayerTurn();
    }

    public void AdvancePlayerTurn()
    {
        // 0 = FIGHT, 1 = ACT, 2 = ITEM, 3 = SPARE
        int action = playerSelection[1];
        int targetIndex = playerSelection[0];
        EnemyBase targetEnemy = enemies[0];
        bool skip = false;

        fightingThisRound = false;
        sparingThisRound = false;
        state = -1;
        switch (action)
        {
            case 0: // FIGHT
                fightingThisRound = true;
                target = Instantiate(Resources.Load<GameObject>("battle/FightTarget"));
                target.GetComponent<FightTarget>().SetEnemies(targetEnemy);
                target.GetComponent<FightTarget>().SetAttackers(true);
                state = 7; 
                skip = false;
                break;

            case 1: // ACT
                Destroy(selObj);
                string[] actDialogue = targetEnemy.PerformAct(playerSelection[2]);
                curDiag = 0;
                skip = false;
                finalDiag = actDialogue.Length - 1;
                FightAfterZ = true;
                StartText(actDialogue[curDiag], new Vector2(-4f, -95f), "snd_txtbtl");
                break;

            case 2: // ITEM
                string[] itemDialogue = Items.ItemUse(gm.GetItem(ItemIndex)).Split('}');
                skip = false;
                gm.UseItem(ItemIndex); 
                curDiag = 0;
                finalDiag = itemDialogue.Length - 1;
                FightAfterZ = true;
                StartText(itemDialogue[curDiag], new Vector2(-4f, -95f), "snd_txtbtl");
                break;

            case 3: // SPARE
                if (targetEnemy.CanSpare() && !targetEnemy.IsDone())
                {
                    targetEnemy.Spare();
                    sparingThisRound = true;
                    diag = new string[] { "* You spared the enemy!" };
                }
                else
                {
                    diag = new string[] { "* But the enemy's name wasn't yellow..." };
                }
                curDiag = 0;
                skip = true;
                finalDiag = diag.Length - 1;
                StartText(diag[curDiag], new Vector2(-4f, -95f), "snd_txtbtl");
                break;

            default:
                AdvanceToEnemyTurn();
                return;
        }

        if (AllEnemiesDone())
        {
            EndNormalFight(false, "");
            return;
        }

        if (!fightingThisRound && skip)
            AdvanceToEnemyTurn();
    }
    public virtual void AdvanceToEnemyTurn()
    {
        if (boxText.Exists())
        {
            boxText.DestroyOldText();
        }
        soul.GetComponent<SpriteRenderer>().enabled = true;
        if (diag == null || buttonIndex == 0 || buttonIndex == 3)
        {
            diag = new string[1] { "" };
            curDiag = 0;
        }
        if (AllEnemiesDone())
        {
            EndNormalFight(customMessage: false, "");
            return;
        }
        int num = -1;
        for (int i = 0; i < enemies.Length; i++)
        {
            if (!enemies[i].IsDone())
            {
                enemies[i].Chat();
                if (num == -1)
                {
                    num = i;
                }
            }
        }
        curAtk = AttackSpawner.GetAttack(enemies[0].GetNextAttack());
        state = 4;
        bb.StartMovement(curAtk.GetBoardSize(), curAtk.GetBoardPos());
        soul.transform.position = curAtk.GetSoulPos();
    }

    public void ForceNoSpare()
    {
        sparingThisRound=false;
        sparingThisRound = false;
    }

    public void ForceNoFight()
    {
        fightingThisRound = false;
    }
    
    public void StartText(string diag, Vector2 pos, string sound)
    {
        string[] array = diag.Split('`');
        if (boxText.Exists())
        {
            ResetText();
        }
        if (array.Length > 1 && !array[0].StartsWith("sounds/"))
        {
            boxPortrait = new GameObject("BoxPortrait", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            boxPortrait.transform.SetParent(GameObject.Find("BattleCanvas").transform);
            Sprite sprite = Resources.Load<Sprite>("overworld/npcs/portraits/spr_" + array[0] + "_0");
            boxPortrait.sizeDelta = new Vector2(sprite.rect.width / 24f, sprite.rect.height / 24f);
            boxPortrait.GetComponent<Image>().sprite = sprite;
            boxPortrait.localPosition = new Vector2(-218f, 40f) + pos;
            pos += new Vector2(108f, 0f);
        }
        if (array.Length > 1 && array[array.Length - 2].StartsWith("snd_"))
        {
            sound = array[array.Length - 2];
        }
        boxText.StartText(array[array.Length - 1], pos, sound, 0, "DTM-Mono");
        if ((UTInput.GetButton("X")) && (state == 0 || state == 3 || state == 10))
        {
            boxText.SkipText(state != 0);
        }
        boxText.GetText().lineSpacing = 1.025f;
    }

    public void ResetText()
    {
        if ((bool)boxPortrait)
        {
            UnityEngine.Object.Destroy(boxPortrait.gameObject);
        }
        boxText.DestroyOldText();
    }

    public TextUT GetBattleText()
    {
        return boxText;
    }

    private bool AllEnemiesDone()
    {
        bool result = true;
        EnemyBase[] array = enemies;
        for (int i = 0; i < array.Length; i++)
        {
            if (!array[i].IsDone())
            {
                result = false;
            }
        }
        return result;
    }
    
    public void EndNormalFight(bool customMessage, string message)
    {
        int num = 0;
        int num2 = 0;
        int num3 = (int)gm.GetFlag(125);
        bool flag = false;
        endState = 2;
        EnemyBase[] array = enemies;
        foreach (EnemyBase enemyBase in array)
        {
            if (enemyBase.IsKilled())
            {
                num3++;
                endState = 1;
            }
            if (enemyBase.IsDone())
            {
                num += enemyBase.GetFinalEXP();
            }
            if (enemyBase.IsSpared())
            {
                flag = true;
            }
            num2 += enemyBase.GetGold();
        }
        if (gm.GetEXP() + num > 99999)
        {
            num = 99999 - gm.GetEXP();
        }
        if (flag && endState == 1)
        {
            endState = 3;
        }
        gm.SetFlag(125, num3);
        soul.GetComponent<SpriteRenderer>().enabled = false;
        StopMusic();
        string text = "* YOU WON!\n* You earned " + num + " XP and " + num2 + " gold.";
        int lV = gm.GetLV();
        gm.AddEXP(num);
        gm.AddGold(num2);
        if (gm.GetLV() > lV)
        {
            gm.PlayGlobalSFX("sounds/snd_levelup");
            partyPanels.UpdateLV();
            text += "\n* Your LOVE increased.";
        }
        partyPanels.UpdateHP(gm.GetHP());
        if (customMessage)
        {
            text = message;
        }
        StartText(text, new Vector2(-4f, -95f), "snd_txtbtl");
        state = 10;
    }
    public virtual void DoSOULSparkle()
    {
        if (!didSoulSparkle)
        {
            didSoulSparkle = true;
            UnityEngine.Object.Instantiate(Resources.Load<GameObject>("vfx/EyeFlashSparkle"), soul.transform.position, Quaternion.identity);
        }
    }

    public bool[] GetDefendingMembers()
    {
        return defending;
    }

    public int[] GetRevivalTurns()
    {
        return revivalTurns;
    }

    public bool IsSeriousMode()
    {
        return isBoss;
    }

    public int GetState()
    {
        return state;
    }

    public int GetCurrentStringNum()
    {
        return curDiag;
    }
    
    private List<int> GetItemListPerTurn()
    {
        return gm.GetItemList();
    }
    public void ChangeHP()
    {
        partyPanels.UpdateHP(gm.GetHP());
    }
}
