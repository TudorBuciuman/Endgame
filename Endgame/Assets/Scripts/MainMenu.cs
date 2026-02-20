using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class MainMenu : SelectableUIComponent
{
    private GameObject cvs;

    private GameManager gm;

    private GameObject main;

    private GameObject pinfo;

    private GameObject newLayer;

    private GameObject itemOptions;

    private GameObject HealthStats;

    private bool usingTextBox;

    private bool isAlone;

    //private ActionPartyPanels panels;

    private int menuOffset;

    private int itemIndex;

    private int partyMemberIndex;

    private bool axisDown;

    private int idleFrames;

    private bool statMenuOpen;

    private bool itemsMenuOpen;

    private bool ActsMenuOpen;

    private AudioSource aud;

    private bool quitting;

    private bool returnPlayerControl = true;

    private bool bnp;

    private float currentPosition;

    private GameObject gameObjectToSpawn;

    private ActionPartyPanels panels;


    private void Awake()
    {
        menuOffset = 0;
        cvs = GameObject.Find("Canvas");
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        aud = base.transform.GetComponent<AudioSource>();
        aud.transform.SetParent(base.transform);
        usingTextBox = false;
        quitting = false;
        bnp = true;
        gm.SetMenu(true);
    }
    
    private void Update()
    {
        usingTextBox = false;
        isAlone = true;
        if (gm.canMove)
        {
            gm.SetMenu(false);
            gm.canMove = true;
            gm.canInteract = true;
            Destroy(pinfo);
            Destroy(main);
            
            Destroy(this.gameObject);
        }
        if (!usingTextBox)
        {
            if (isAlone)
            {
                idleFrames++;
            }
           
            if ((statMenuOpen || itemsMenuOpen || ActsMenuOpen) && !gm.MenuDis())
            {
                if (Input.GetKeyDown(KeyCode.X)) 
                {
                    if (statMenuOpen)
                    {
                        Destroy(newLayer.gameObject);
                        statMenuOpen = false;
                    }
                    else if (itemsMenuOpen)
                    {
                        Destroy(newLayer.gameObject);
                        itemsMenuOpen = false;
                    }
                    else if (ActsMenuOpen)
                    {
                        Destroy(newLayer.gameObject);
                        ActsMenuOpen = false;
                    }
                    GameObject.Find("MainMenu").GetComponent<Selection>().Enable();

                }
            }
            else if ((Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.X)) && !gm.MenuDis())
            {
                gm.SetMenu(false);
                Destroy(pinfo);
                Destroy(main);
                gm.canMove = true;
                gm.canInteract = true;
                Destroy(this.gameObject);
            }
            if (axisDown && UTInput.GetAxis("Horizontal") == 0f)
            {
                axisDown = false;
            }
        }

    }
    
    private void LateUpdate()
    {
        currentPosition = Mathf.Lerp(currentPosition, 0f, 0.5f);
        GameObject[] menuObjectArray = GetMenuObjectArray();
        foreach (GameObject gameObject in menuObjectArray)
        {
            if ((bool)gameObject)
            {
                gameObject.transform.localPosition = new Vector3(currentPosition, gameObject.transform.localPosition.y);
            }
        }
    }

    public override void MakeDecision(Vector2 index, int id)
    {
        if (id == 0)
        {
            isAlone = false;
            idleFrames = 0;
            if ((bool)panels)
            {
                Object.Destroy(panels.gameObject);
            }
            main.GetComponent<Selection>().Disable();
            if (index[0] + (float)menuOffset == 0f)
            {
                CreateItemsMenu();
            }
            else if (index[0] + (float)menuOffset == 1f)
            {
                CreateStatsMenu();
                partyMemberIndex = 0;
            }
            else if (index[0] + (float)menuOffset == 2f)
            {
                CreateActsMenu();
            }
        }
        if (id == 1)
        {
            itemIndex = (int)index[0];
            newLayer.GetComponent<Selection>().Disable();
            itemOptions.GetComponent<Selection>().Enable();
            itemOptions.GetComponent<Selection>().Stick();
        }
        if (id == 2)
        {
            //itemsMenuOpen = false;
                gm.SetMenuDisabled(true);
                TextBox textBox = TextDecision();
                List<string> list = new List<string>();
                List<string> list2 = new List<string>();
                List<int> list3 = new List<int>();
                List<string> list4 = new List<string>();
                if (index[1] == 0f)
                {
                    string[] array2 = Items.ItemUse(gm.GetItem(itemIndex)).Split('}');
                    for (int i = 0; i < array2.Length; i++)
                    {
                        string[] array3 = array2[i].Split('`');
                        if (array3.Length > 1)
                        {
                            list4.Add(array3[0]);
                            if (array3[array3.Length - 2].StartsWith("snd"))
                            {
                                list2.Add(array3[array3.Length - 2]);
                            }
                            else
                            {
                                list2.Add("snd_text");
                            }
                        }
                        else
                        {
                            list4.Add("");
                            list2.Add("snd_text");
                        }
                        list.Add(array3[array3.Length - 1]);
                        list3.Add(0);
                    }
                    gm.UseItem(itemIndex);

                }
                else if (index[1] == 1f)
                {
                //info
                    string[] array4 = Items.ItemDescription(gm.GetItem(itemIndex)).Split('}');
                    foreach (string item in array4)
                    {
                        list.Add(item);
                    }
                    //then the menu dissapears
                }
                else if (index[1] == 2f)
                {
                //drop
                    list.Add(Items.ItemDrop(gm.GetItem(itemIndex)));
                    gm.RemoveItem(itemIndex);
                }
                if (list2.Count == 0)
                {
                    textBox.CreateBox(list.ToArray());
                }
                else
                {
                    textBox.CreateBox(list.ToArray(), list2.ToArray(), list3.ToArray(), list4.ToArray());
                }
        }
        if (id == 3)
        {
            Debug.Log("scene");
        }
        if (id == 4)
        {
            if (index[1] == 1f)
            {
                Object.Destroy(newLayer);
                Object.Destroy(GameObject.Find("QuitProtection"));
                Application.Quit();
            }
            else
            {
                Object.Destroy(GameObject.Find("QuitProtection"));
                TextDecision().CreateBox(new string[1] { "* Stay determined." });
            }
        }
        if (id == 5)
        {
            Object.Destroy(newLayer);
            Object.Destroy(base.gameObject);
        }
        if (id != 6)
        {
            return;
        }
        TextBox textBox5 = TextDecision();
        List<string> list8 = new List<string>();
        List<string> list9 = new List<string>();
        List<int> list10 = new List<int>();
        List<string> list11 = new List<string>();
        if (index[1] == 1f)
        {
            partyMemberIndex = (2);
        }
        else
        {
            partyMemberIndex = (int)index[1];
        }
        string[] array20 = Items.ItemUse(gm.GetItem(itemIndex)).Split('}');
        for (int m = 0; m < array20.Length; m++)
        {
            string[] array21 = array20[m].Split('`');
            if (array21.Length > 1)
            {
                list11.Add(array21[0]);
                if (array21[array21.Length - 2].StartsWith("snd"))
                {
                    list9.Add(array21[array21.Length - 2]);
                }
                else
                {
                    list9.Add("snd_text");
                }
            }
            else
            {
                list11.Add("");
                list9.Add("snd_text");
            }
            list8.Add(array21[array21.Length - 1]);
            list10.Add(0);
        }
        //gm.UseItem(partyMemberIndex, itemIndex);
        textBox5.CreateBox(list8.ToArray(), list9.ToArray(), list10.ToArray(), list11.ToArray());
    }
    
    public void CreateMainMenu()
    {
        main = new GameObject("MainMenu");
        main.layer = 5;
        main.AddComponent<RectTransform>();
        main.transform.SetParent(cvs.transform);
        main.AddComponent<UIBackground>();
        bnp = false;
        main.GetComponent<UIBackground>().CreateElement("mmenu", new Vector2(bnp ? (-212) : (-217), -2f), new Vector2(bnp ? 152 : 142, 148f));
        main.AddComponent<Selection>();
            string[,] array = new string[4, 1]
            {
                { "ITEM" },
                { "STAT" },
                { "SCENE" },
                { "" }
            };
            main.GetComponent<Selection>().CreateSelections(array, new Vector2(-236f, -59f), new Vector2(0f, -36f), new Vector2(-19f, 94f), "DTM-Sans", useSoul: true, makeSound: true, this, 0);
            main.GetComponent<Selection>().SetWrap(wrap: true);
    
        pinfo = new GameObject("PlayerInfo");
        pinfo.layer = 5;
        pinfo.AddComponent<RectTransform>();
        pinfo.transform.SetParent(cvs.transform);
        pinfo.AddComponent<UIBackground>();
        int num = 0;
        
        if (GameObject.Find("Player").transform.position.y - GameObject.Find("Camera").transform.position.y < -0.9f)
        {
            num = 270;
        }
        pinfo.GetComponent<UIBackground>().CreateElement("pinfo", new Vector2(bnp ? (-212) : (-217), 133 - num), new Vector2(bnp ? 152 : 142, 110f));
        GameObject gameObject = Object.Instantiate(Resources.Load<GameObject>("ui/SelectionBase"), pinfo.transform.GetChild(0).transform);
        gameObject.transform.localPosition = new Vector2(-57f, -64f);
        gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        gameObject.GetComponent<Text>().text = "Pawn";
        gameObject.transform.SetParent(pinfo.transform);
        HealthStats = Object.Instantiate(Resources.Load<GameObject>("ui/SelectionBase"), pinfo.transform.GetChild(0).transform);
        HealthStats.transform.localPosition = new Vector2(-57f, -103f);
        HealthStats.transform.localScale = new Vector3(1f, 1f, 1f);
        HealthStats.GetComponent<Text>().font = Util.PackManager().GetFont(Resources.Load<Font>("fonts/hud"), "hud");
        HealthStats.GetComponent<Text>().fontSize = 16;
        HealthStats.GetComponent<Text>().lineSpacing = 3f;
        HealthStats.GetComponent<Text>().text = "lv  " + gm.GetLV() + "\nhp  " + gm.GetHP() + "/" + gm.GetMaxHP() + "\ng   " + gm.GetGold();
        HealthStats.transform.SetParent(pinfo.transform);
        aud.clip = Resources.Load<AudioClip>("sounds/snd_menumove");
        aud.Play();
        isAlone = true;
    }
    public void CreateItemsMenu()
    {
        itemsMenuOpen = true;
        newLayer = new GameObject("Items");
        newLayer.layer = 5;
        newLayer.AddComponent<RectTransform>();
        newLayer.transform.SetParent(cvs.transform);
        newLayer.AddComponent<UIBackground>();
        newLayer.GetComponent<UIBackground>().CreateElement("items", new Vector2(bnp ? 42 : 41, 7f), new Vector2(bnp ? 344 : 346, 362f));
        if (bnp)
        {
            Object.Instantiate(Resources.Load<GameObject>("ui/bnpicons/ItemIcon"), newLayer.transform);
        }
        int num = 0;
        
        for (int i = 0; i < 8 && gm.GetItem(i) != -1; i++)
        {
            num++;
        }
        

        if (num > 0)
        {
            string[,] array = new string[num, 1];
            for (int j = 0; j < num; j++)
            {
                array[j, 0] = Items.ItemName(gm.GetItem(j));
            }
            newLayer.AddComponent<Selection>();
            newLayer.GetComponent<Selection>().CreateSelections(array, new Vector2(-88f, 49f), new Vector2(0f, -32f), new Vector2(-15f, 94f), "DTM-Sans", useSoul: true, makeSound: true, this, 1);
            newLayer.GetComponent<Selection>().SetWrap(wrap: true);
        }
        itemOptions = new GameObject("Items");
        itemOptions.layer = 5;
        itemOptions.AddComponent<RectTransform>();
        itemOptions.transform.SetParent(newLayer.transform);
        itemOptions.transform.localScale = new Vector3(1f, 1f, 1f);
        itemOptions.AddComponent<Selection>();
        itemOptions.GetComponent<Selection>().CreateSelections(new string[1, 3] { { "USE", "INFO", "DROP" } }, new Vector2(-88f, -231f), new Vector2(96f, -32f), new Vector2(-15f, 94f), "DTM-Sans", useSoul: true, makeSound: true, this, 2);
        itemOptions.GetComponent<Selection>().Disable();
        itemOptions.GetComponent<Selection>().SetWrap(wrap: true);
    }
    public void CreateStatsMenu()
    {
        statMenuOpen = true;
        newLayer = new("Stats");
        newLayer.layer = 5;
        newLayer.AddComponent<RectTransform>();
        newLayer.transform.SetParent(cvs.transform);
        newLayer.AddComponent<UIBackground>();
        newLayer.GetComponent<UIBackground>().CreateElement("stats", bnp ? new Vector2(46f, -16f) : new Vector2(41f, -21f), bnp ? new Vector2(352f, 408f) : new Vector2(346f, 418f));
        if (bnp)
        {
            Transform obj = Object.Instantiate(Resources.Load<GameObject>("ui/bnpicons/StatIcons"), newLayer.transform).transform;
            int num = gm.GetLV() / 5;
        }
        GameObject gameObject = Object.Instantiate(Resources.Load<GameObject>("ui/SelectionBase"), base.transform.position, Quaternion.identity);
        gameObject.transform.SetParent(newLayer.transform);
        gameObject.transform.localPosition = new Vector3(-104f, -251f);
        gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(300f, 406f);
        GameObject gameObject2 = Object.Instantiate(gameObject);
        GameObject gameObject3 = Object.Instantiate(gameObject);
        gameObject2.transform.SetParent(newLayer.transform);
        gameObject2.transform.localPosition = new Vector3(64f, -251f);
        gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
        gameObject3.transform.SetParent(newLayer.transform);
        gameObject3.transform.localPosition = new Vector3(-104f, -509f);
        gameObject3.transform.localScale = new Vector3(1f, 1f, 1f);

        string text = "Pawn";
        string text2 = gm.GetHP().ToString();
        string text3 = gm.GetMaxHP().ToString();
        string text4 = "";
        string text5 = gm.GetATKRaw().ToString();
        string text6 = gm.GetDEFRaw().ToString();
        string text8 = (gm.GetATK() - gm.GetATKRaw()).ToString();
        string text9 = (gm.GetDEF() - gm.GetDEFRaw()).ToString();

        gameObject.GetComponent<Text>().text = "\"" + text + "\"\n" + text4 + "\nLV  " + gm.GetLV() + "\nHP  " + text2 + " / " + text3 + "\n\nAT  " + text5 + " (" + text8 + ")\nDF  " + text6 + " (" + text9 + ")";
        gameObject.GetComponent<Text>().lineSpacing = 1f;
        gameObject2.GetComponent<Text>().text = "\n\n\n\n\nEXP: " + gm.GetEXP() + "\nNEXT: " + (gm.GetLVExp() - gm.GetEXP()).ToString();
        gameObject2.GetComponent<Text>().lineSpacing = 1f;
        string weapon = Items.ItemName(gm.GetWeapon());
        string armor = Items.ItemName(gm.GetArmor());
        if (weapon.Length > 13)
            weapon = Items.ShortItemName(gm.GetWeapon());
        if (armor.Length > 13)
            armor = Items.ShortItemName(gm.GetArmor());
        gameObject3.GetComponent<Text>().text = "WEAPON: " + weapon + "\nARMOR: " + armor + "\nGOLD: " + gm.GetGold().ToString();
        gameObject3.GetComponent<Text>().lineSpacing = 1f;
    }
    public void CreateActsMenu()
    {
        statMenuOpen = true;
        newLayer = new("Scene");
        newLayer.layer = 5;
        newLayer.AddComponent<RectTransform>();
        newLayer.transform.SetParent(cvs.transform);
        newLayer.AddComponent<UIBackground>();
        newLayer.GetComponent<UIBackground>().CreateElement("stats", bnp ? new Vector2(46f, -16f) : new Vector2(41f, -21f), bnp ? new Vector2(352f, 408f) : new Vector2(346f, 418f));
  
        GameObject gameObject = Object.Instantiate(Resources.Load<GameObject>("ui/SelectionBase"), base.transform.position, Quaternion.identity);
        gameObject.transform.SetParent(newLayer.transform);
        gameObject.transform.localPosition = new Vector3(-104f, -251f);
        gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        GameObject gameObject2 = Object.Instantiate(Resources.Load<GameObject>("ui/SelectionBase"), base.transform.position, Quaternion.identity);
        gameObject2.transform.SetParent(newLayer.transform);
        gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
        gameObject2.transform.localPosition = new Vector3(-104f, -251f);
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(300f, 406f);
        gameObject2.GetComponent<RectTransform>().sizeDelta = new Vector2(300f, 406f);
        gameObject2.GetComponent<Text>().font = Util.PackManager().GetFont(Resources.Load<Font>("fonts/hud"), "hud");
        gameObject2.GetComponent<Text>().fontSize = 16;
        string text, text2, text3, text4;
        if (gm.GetAct() == 1)
        {
            gameObject2.GetComponent<Text>().text = "Act 1";

            text = "FIGHTING";
            text2 = "Light beamed into \nthe world,";
            text3 = "but men and women";
            text4 = "ran towards the darkness.";
            gameObject.GetComponent<Text>().text = "\n" + text + "\n" + "\n" + "\n" + text2 + "\n" + text3 + "\n" + text4;

        }
        else if(gm.GetAct()==2)
        {
            gameObject2.GetComponent<Text>().text = "Act 2";

            text = "RISING";
            text2 = "Pride always preludes \nthe crash";
            text3 = "...the bigger the ego";
            text4 = "the harder the fall!";
            gameObject.GetComponent<Text>().text = "\n" + text + "\n" + "\n" + "\n" + text2 + "\n" + text3 + "\n" + text4;

        }
        else {
            gameObject2.GetComponent<Text>().text = "Act 3";

            text = "FALLING";
            text2 = "Mere mortals \ncannot ruin \ntheir own lives.";
            text3 = "Who will give me wings,";
            text4 = "I ask,\n\"wings like a dove\"";
            gameObject.GetComponent<Text>().text = "\n" + text + "\n" + "\n" + text2 + "\n" + text3 + "\n" + text4;

        }
        gameObject.GetComponent<Text>().lineSpacing = 1f;
    }
    public void RewriteHealth()
    {
        HealthStats.GetComponent<Text>().text = "lv  " + gm.GetLV() + "\nhp  " + gm.GetHP() + "/" + gm.GetMaxHP() + "\ng   " + 10;
    }
    public override void CancelControlReturn()
    {
        returnPlayerControl = false;
    }

    private GameObject[] GetMenuObjectArray()
    {
        return new GameObject[4]
        {
            main ? main : null,
            pinfo ? pinfo : null,
            newLayer ? newLayer : null,
            panels ? panels.gameObject : null
        };
    }
    
    public void OnDestroy()
    {
        GameObject[] menuObjectArray = GetMenuObjectArray();
        if (bnp)
        {
            GameObject gameObject = new GameObject("MenuSlide");
            try
            {
                gameObject.transform.SetParent(cvs.transform);
            }
            catch
            {
                if ((bool)gameObject)
                {
                    Object.Destroy(gameObject);
                }
                return;
            }
            if ((bool)itemOptions)
            {
                Object.Destroy(itemOptions.GetComponent<Selection>());
            }
            GameObject[] array = menuObjectArray;
            foreach (GameObject gameObject2 in array)
            {
                if ((bool)gameObject2)
                {
                    Selection component = gameObject2.GetComponent<Selection>();
                    if ((bool)component)
                    {
                        Object.Destroy(component);
                    }
                    gameObject2.transform.SetParent(gameObject.transform);
                }
            }
            //gameObject.AddComponent<HDMenuSlide>().Slide(-640f);
        }
        else
        {
            GameObject[] array = menuObjectArray;
            foreach (GameObject gameObject3 in array)
            {
                if ((bool)gameObject3)
                {
                    Object.Destroy(gameObject3);
                }
            }
        }
        if ((bool)panels)
        {
            Object.Destroy(panels.gameObject);
        }
        gm.SetMenuDisabled(false);
            gm.canMove = true;
            gm.SetMenu(false);
        if ((bool)gameObjectToSpawn)
        {
            Object.Instantiate(gameObjectToSpawn, GameObject.Find("Canvas").transform, worldPositionStays: false);
        }
    }
    
    public TextBox TextDecision()
    {
        usingTextBox = true;
        Object.Destroy(newLayer);
        return base.gameObject.AddComponent<TextBox>();
    }
    
}
