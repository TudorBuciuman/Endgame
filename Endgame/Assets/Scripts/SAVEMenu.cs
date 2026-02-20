using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class SaveMenu : UIComponent
{
    private int state;

    private int saveSlot = -1;

    private int overwriteSaveSlot;

    private SAVEFile saves;

    private bool saveSlotsTaken;

    private Transform mainbox;

    private Transform savefiles;

    private Transform overwrite;

    private Transform soul;

    private int index;

    private bool holdingAxis;

    private bool tsMode;

    private bool confirmQuit;

    private bool returnControl = true;

    private GameManager gm;

    private int tsExit;

    private Color borderColor = Color.white;

    private Color selectionColor = new Color(1f, 1f, 0f);
    
    private void Awake()
    {
        gm = Util.GameManager();
        borderColor = UIBackground.borderColors[(int)gm.GetFlag(223)];
        selectionColor = Selection.selectionColors[(int)gm.GetFlag(223)];
        Image[] componentsInChildren = GetComponentsInChildren<Image>();
        foreach (Image image in componentsInChildren)
        {
            if (image.color == Color.white && image.gameObject.name != "TimeIcon")
            {
                image.color = borderColor;
            }
        }
        Text[] componentsInChildren2 = GetComponentsInChildren<Text>();
        foreach (Text text in componentsInChildren2)
        {
            if (text.color == new Color(1f, 1f, 0f))
            {
                text.color = selectionColor;
            }
        }
        tsMode = (int)gm.GetFlag(94) == 1;
        //saveSlot = gm.GetFileID();
        mainbox = transform.Find("MainBox");
        //savefiles = base.transform.Find("SaveFiles");
        //overwrite = base.transform.Find("Overwrite");
        soul = base.transform.Find("SOUL");
        soul.GetComponent<Image>().color = Color.red; 
        UpdateAllText();
        if (!tsMode)
        {
            return;
        }
        mainbox.localPosition = Vector3.zero;
        base.transform.Find("MainBox").localPosition = new Vector3(1000f, 0f);
        soul.localPosition = new Vector3(19f, 43f);
    }

    private void Update()
    {
        if (state <= 1)
        {
            if (tsMode)
            {
                mainbox.Find("RoomBox").transform.localPosition = Vector3.Lerp(mainbox.Find("RoomBox").transform.localPosition, new Vector3(0f, 40f), 0.2f);
            }
        }
        if (holdingAxis && UTInput.GetAxis("Horizontal") == 0f && UTInput.GetAxis("Vertical") == 0f)
        {
            holdingAxis = false;
        }
        if (state == 0)
        {
            if (tsMode)
            {
                if (UTInput.GetAxis("Vertical") != 0f && !holdingAxis)
                {
                    if (confirmQuit)
                    {
                        holdingAxis = true;
                        confirmQuit = false;
                        mainbox.Find(index.ToString()).GetComponent<Text>().text = "Quit Game";
                        mainbox.Find(index.ToString()).GetComponent<Text>().color = Color.white;
                    }
                    else
                    {
                        index = (index - (int)UTInput.GetAxis("Vertical")) % 4;
                        if (index < 0)
                        {
                            index = 3;
                        }
                        holdingAxis = true;
                    }
                }
                soul.localPosition = new Vector3(19f, 43 - 30 * index);
            }
            else
            {
                if (UTInput.GetAxis("Horizontal") != 0f && !holdingAxis)
                {
                    index = (index + (int)UTInput.GetAxis("Horizontal")) % 2;
                    if (index < 0)
                    {
                        index = 1;
                    }
                    holdingAxis = true;
                }
                soul.localPosition = mainbox.Find(index.ToString()).localPosition + new Vector3(-19f, 96f);
            }
            if ((UTInput.GetButtonDown("Z") && index == 1))
            {
                gm.SetMenuDisabled(false);
                if (confirmQuit)
                {
                    confirmQuit = false;
                    mainbox.Find(index.ToString()).GetComponent<Text>().text = "Quit Game";
                    mainbox.Find(index.ToString()).GetComponent<Text>().color = Color.white;
                }
                else
                {
                    Object.Destroy(base.gameObject);
                }
            }
            else if (index == 0 && UTInput.GetButtonDown("Z"))
            {
                gm.PlayGlobalSFX("sounds/snd_select");
                //mainbox.transform.localPosition = new Vector3(1000f, 0f);
                //savefiles.transform.localPosition = Vector3.zero;
                //base.transform.Find("Background").GetComponent<Image>().enabled = true;
                state = 1;
                UpdateSaveFileColors();
                SaveFile();
                holdingAxis = true;

            }
        }
        else if (state == 1)
        {
            if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                overwriteSaveSlot = index;
                if (index == saveSlot || !saveSlotsTaken)
                {
                    SaveFile();
                    return;
                }
                index = 0;
                LoadOverwrite();
                soul.localPosition = overwrite.Find(index.ToString()).localPosition + new Vector3(-20f, 16f);
            }
        }
        else if (state == 3)
        {
            if (((Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) )))
            {
                Debug.Log(3);
                gm.canMove = true;
                gm.canInteract = true;
                gm.SetMenuDisabled(false);
                Destroy(this.gameObject);
            }
        }
    }

    private void LoadOverwrite()
    {
        overwrite.transform.localPosition = Vector3.zero;
        overwrite.Find("Confirm").GetComponent<Text>().text = "Overwrite Slot " + (overwriteSaveSlot + 1) + "?";
        overwrite.Find("Name").GetComponent<Text>().text = gm.GetPlayerName();
        overwrite.Find("Room").GetComponent<Text>().text = MapInfo.GetMapName(gm.GetCurrentZone());
        overwrite.Find("LV").GetComponent<Text>().text = "LV " + gm.GetLV();
        overwrite.Find("Time").GetComponent<Text>().text = gm.GetFormattedUpdatedPlayTime();
        overwrite.Find("NameOld").GetComponent<Text>().text = saves.name;
        overwrite.Find("RoomOld").GetComponent<Text>().text = MapInfo.GetMapName(saves.zone);
        overwrite.Find("LVOld").GetComponent<Text>().text = "LV " + gm.GetLV(saves.exp);
        overwrite.Find("TimeOld").GetComponent<Text>().text = gm.GetFormattedPlayTimeFromTime(saves.playTime);
        overwrite.Find("0").GetComponent<Text>().color = selectionColor;
        overwrite.Find("1").GetComponent<Text>().color = Color.white;
        Image[] componentsInChildren = savefiles.GetComponentsInChildren<Image>();
        foreach (Image image in componentsInChildren)
        {
            if (image.gameObject.name != "Background" && image.color != Color.black)
            {
                image.color = new Color(borderColor.r * 0.2f, borderColor.g * 0.2f, borderColor.b * 0.2f);
            }
        }
        Text[] componentsInChildren2 = savefiles.GetComponentsInChildren<Text>();
        for (int i = 0; i < componentsInChildren2.Length; i++)
        {
            componentsInChildren2[i].color = new Color32(51, 51, 51, byte.MaxValue);
        }
    }

    private void SaveFile()
    {
        gm.PlayGlobalSFX("sounds/snd_save");
        gm.SaveFile(savepoint: true);
        soul.GetComponent<Image>().enabled = false;
        state = 3;
    }

    private void UpdateSaveFileColors()
    {
        
        Text[] componentsInChildren;
        componentsInChildren = transform.Find("MainBox").GetComponentsInChildren<Text>();
        transform.Find("MainBox").Find("Room").GetComponent<Text>().text = MapInfo.GetMapName(gm.GetCurrentZone());
        transform.Find("MainBox").Find("Time").GetComponent<Text>().text = gm.GetCurrentPlayTime();
        transform.Find("MainBox").Find("LV").GetComponent<Text>().text = "LV "+gm.GetLV(gm.GetEXP()).ToString();
        transform.Find("MainBox").Find("0").GetComponent<Text>().text = "File Saved";
        transform.Find("MainBox").Find("1").GetComponent<Text>().text = "";

        for (int j = 0; j < componentsInChildren.Length; j++)
        {
            componentsInChildren[j].color = Color.yellow;
        }
        soul.transform.position = new Vector2(0, 10);
    }

    private void UpdateAllText()
    {
        mainbox.Find("Name").GetComponent<Text>().text = "Pawn";
        mainbox.Find("Time").GetComponent<Text>().text = gm.GetFormattedUpdatedPlayTime();

            string path = Path.Combine(Application.persistentDataPath, "SAVE.sav");
            new BinaryFormatter();
            if (File.Exists(path))
            {
                try
                {
                    using (FileStream fs = File.Open(path, FileMode.Open))
                    {
                        SAVEFileIO.ReadFile(ref saves, fs);
                        saveSlotsTaken = true;
                        transform.Find("MainBox").Find("Name").GetComponent<Text>()
                            .text = "Pawn";
                        transform.Find("MainBox").Find("Time").GetComponent<Text>()
                            .text = gm.GetFormattedPlayTimeFromTime(saves.playTime);
                        transform.Find("MainBox").Find("Room").GetComponent<Text>()
                            .text = MapInfo.GetMapName(saves.zone);
                        transform.Find("MainBox").Find("LV").GetComponent<Text>()
                            .text = "LV " + gm.GetLV(saves.exp);
                    }
                }
                catch
                {
                    Text[] componentsInChildren = savefiles.Find("MainBox").GetComponentsInChildren<Text>();
                    for (int j = 0; j < componentsInChildren.Length; j++)
                    {
                        componentsInChildren[j].enabled = false;
                    }
                    transform.Find("MainBox").Find("CenterText").GetComponent<Text>()
                        .enabled = true;
                }
            }
            else
            {
            transform.Find("MainBox").Find("Name").GetComponent<Text>()
                        .text = "Pawn";
            transform.Find("MainBox").Find("Time").GetComponent<Text>()
                .text = "0:00";
            transform.Find("MainBox").Find("Room").GetComponent<Text>()
                .text = "--";
            transform.Find("MainBox").Find("LV").GetComponent<Text>()
                .text = "LV 0";
        }
    }

    public override void CancelControlReturn()
    {
        returnControl = false;
    }
    
}
