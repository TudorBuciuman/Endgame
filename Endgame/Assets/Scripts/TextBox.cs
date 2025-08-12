using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBox : UIComponent
{
    private string[] dialog;

    private int boxPos;

    private Vector2 mainTextPos;

    private GameObject menu;

    private TextUT text;

    private int[] textSpeeds;

    private string[] textSounds;

    private Dictionary<int, Queue<string>> textRemarks;

    private string font;

    private int currentString;

    private int lastString;

    private bool firstString;

    private string[] portraits;

    private Vector3[] portTextLocations;

    private GameObject portrait;

    private int currentPortrait;

    private int portFrames;

    private TextRemark remark;

    private Vector3[][] remarkLocations;

    private int toNewTextFrames;

    private bool giveControl;

    private bool isControllable;

    private bool canSkip;

    private bool disabled;

    private bool selectionEnabled;

    private bool canLoadSelection;

    private bool forceAdvance;

    private string lastSound = "snd_text";

    private int lastSpeed;

    private int portraitSpeed = 6;

    private bool infinitePortrait;

    private void Awake()
    {
        canSkip = true;
        lastString = 0;
        firstString = false;
        isControllable = true;
        disabled = false;
        portFrames = -1;
        portTextLocations = new Vector3[6]
        {
            new Vector2(4f, 142f),
            new Vector2(4f, -168f),
            new Vector2(120f, 142f),
            new Vector2(120f, -168f),
            new Vector2(-217f, 156f),
            new Vector2(-217f, -154f)
        };
        remarkLocations = new Vector3[2][]
        {
            new Vector3[2]
            {
                new Vector3(95f, 142f),
                new Vector3(95f, -168f)
            },
            new Vector3[2]
            {
                new Vector3(165f, 142f),
                new Vector3(165f, -168f)
            }
        };
        font = "DTM-Mono";
        textRemarks = new Dictionary<int, Queue<string>>();
    }

    private void Update()
    {
        if (!text || lastString < 0)
        {
            return;
        }
        if (currentString == 0 && toNewTextFrames == 0)
        {
            if (!PortraitIsEmpty(0))
            {
                toNewTextFrames = 7;
            }
            else
            {
                toNewTextFrames = 9;
            }
        }
        if ((bool)text.GetGameObject())
        {
            if (text.IsPlaying())
            {
                if ((Input.GetKeyDown(KeyCode.X)) && canSkip)
                {
                    text.SkipText();
                }
            }
            else if (textRemarks.ContainsKey(currentString) && !canLoadSelection)
            {
                if (!remark)
                {
                    remark = Object.Instantiate(Resources.Load<GameObject>("ui/TextRemark"), menu.transform).GetComponent<TextRemark>();
                    remark.StartRemark(remarkLocations[portrait ? 1 : 0][boxPos], textRemarks[currentString].Dequeue());
                }
                if (Input.GetKeyDown(KeyCode.X) && remark.CanAdvance())
                {
                    remark.Skip();
                }
                if (!remark.CanAdvance())
                {
                    remark = null;
                    if (textRemarks[currentString].Count == 0)
                    {
                        textRemarks.Remove(currentString);
                    }
                }
            }
            else if ((Input.GetKeyDown(KeyCode.Z) || forceAdvance) && !disabled)
            {
                forceAdvance = false;
                text.DestroyOldText();
                if ((bool)portrait)
                {
                    Object.Destroy(portrait);
                }
                TextRemark[] componentsInChildren = menu.GetComponentsInChildren<TextRemark>();
                for (int i = 0; i < componentsInChildren.Length; i++)
                {
                    Object.Destroy(componentsInChildren[i].gameObject);
                }
                portFrames = -1;
                infinitePortrait = false;
                if (currentString <= lastString)
                {
                    toNewTextFrames = 9;
                    if (!PortraitIsEmpty(currentString) && !PortraitIsEmpty(currentString - 1))
                    {
                        string[] array = portraits[currentString].Split('_');
                        string[] array2 = portraits[currentString - 1].Split('_');
                        if (array[0] != array2[0])
                        {
                            toNewTextFrames = 3;
                        }
                    }
                    else if (!PortraitIsEmpty(currentString) && PortraitIsEmpty(currentString - 1))
                    {
                        toNewTextFrames = 3;
                    }
                    if (Input.GetKey(KeyCode.X))
                    {
                        toNewTextFrames = 9;
                    }
                }
                else if (!selectionEnabled)
                {
                    Object.Destroy(base.gameObject);
                    GameManager.instance.canMove = true;
                    GameManager.instance.canInteract = true;
                }
                else
                {
                    canLoadSelection = true;
                }
            }
        }
        if (portFrames > -1 || infinitePortrait)
        {
            portFrames++;
            if (portFrames == portraitSpeed)
            {
                Sprite sprite = Resources.Load<Sprite>("overworld/npcs/portraits/spr_" + portraits[currentPortrait] + "_1");
                if ((bool)sprite)
                {
                    portrait.GetComponent<Image>().sprite = sprite;
                }
            }
            if (portFrames == portraitSpeed * 2)
            {
                portrait.GetComponent<Image>().sprite = Resources.Load<Sprite>("overworld/npcs/portraits/spr_" + portraits[currentPortrait] + "_0");
                portFrames = 0;
            }
            if (!text.IsPlaying() && !infinitePortrait)
            {
                portrait.GetComponent<Image>().sprite = Resources.Load<Sprite>("overworld/npcs/portraits/spr_" + portraits[currentPortrait] + "_0");
                portFrames = -1;
            }
        }
        if (toNewTextFrames >= 10)
        {
            return;
        }
        toNewTextFrames++;
        if (toNewTextFrames != 10)
        {
            return;
        }
        Vector3 vector = Vector3.zero;
        string theText = "* No text here.";
        string theSound = lastSound;
        int speed = lastSpeed;
        if (currentString < dialog.Length)
        {
            theText = dialog[currentString];
        }
        if (currentString < textSounds.Length)
        {
            theSound = (lastSound = textSounds[currentString]);
        }
        if (currentString < textSpeeds.Length)
        {
            speed = (lastSpeed = textSpeeds[currentString]);
        }
        text.StartText(theText, mainTextPos, theSound, speed, font);
        if (text.GetText().font.name == "sans")
        {
            vector = new Vector3(0f, -4f);
        }
        currentString++;
        if (Input.GetKey(KeyCode.X) && canSkip)
        {
            text.SkipText();
        }
    }



    public void CreateBox(string[] stuffToSay, string[] sound, int[] speed, int location, bool giveBackControl, string[] portraitNames)
    {
        textSounds = sound;
        textSpeeds = speed;
        dialog = stuffToSay;
        lastString = dialog.Length - 1;
        currentString = 0;
        firstString = true;
        boxPos = location;
        portraits = portraitNames;
        GameObject gameObject = GameObject.Find("Canvas");
        menu = new GameObject("TextBox");
        menu.layer = 5;
        menu.AddComponent<RectTransform>();
        menu.transform.SetParent(gameObject.transform);
        menu.AddComponent<UIBackground>();
        Vector2[] array = new Vector2[4]
        {
            new Vector2(1f, 154f),
            new Vector2(1f, -156f),
            new Vector2(4f, 142f),
            new Vector2(4f, -168f)
        };
        menu.GetComponent<UIBackground>().setUpInfo("menu", array[location], new Vector2(578f, 152f));
        menu.GetComponent<UIBackground>().CreateElement();
        menu.AddComponent<AudioSource>();
        menu.AddComponent<AudioSource>();
        menu.AddComponent<TextUT>();
        text = menu.GetComponent<TextUT>();
        mainTextPos = array[location + 2];
        giveControl = giveBackControl;
    }

    public void CreateBox(string[] stuffToSay, string[] sound, int[] speed, int location, bool giveBackControl, string[] portraitNames, string font)
    {
        this.font = font;
        CreateBox(stuffToSay, sound, speed, location, giveBackControl, portraitNames);
    }

    public void CreateBox(string[] stuffToSay, string[] sound, int[] speed, int location, bool giveBackControl)
    {
        string[] array = new string[stuffToSay.Length];
        for (int i = 0; i < stuffToSay.Length; i++)
        {
            array[i] = null;
        }
        CreateBox(stuffToSay, sound, speed, location, giveBackControl, array);
    }

    public void CreateBox(string[] stuffToSay, string sound, int speed, int location, bool giveBackControl)
    {
        string[] array = new string[stuffToSay.Length];
        int[] array2 = new int[stuffToSay.Length];
        for (int i = 0; i < stuffToSay.Length; i++)
        {
            array[i] = sound;
            array2[i] = speed;
        }
        CreateBox(stuffToSay, array, array2, location, giveBackControl);
    }

    public void CreateBox(string[] stuffToSay, string[] sound, int[] speed, bool giveBackControl)
    {
        if (GameObject.Find("Player").transform.position[1] - GameObject.Find("Camera").transform.position[1] < -0.9f)
        {
            CreateBox(stuffToSay, sound, speed, 0, giveBackControl);
        }
        else
        {
            CreateBox(stuffToSay, sound, speed, 1, giveBackControl);
        }
    }

    public void CreateBox(string[] stuffToSay, string[] sound, int[] speed, bool giveBackControl, string[] portraitNames)
    {
        if (GameObject.Find("Player").transform.position[1] - GameObject.Find("Camera").transform.position[1] < -0.9f)
        {
            CreateBox(stuffToSay, sound, speed, 0, giveBackControl, portraitNames);
        }
        else
        {
            CreateBox(stuffToSay, sound, speed, 1, giveBackControl, portraitNames);
        }
    }

    public void CreateBox(string[] stuffToSay, string[] sound, int[] speed)
    {
        if (GameObject.Find("Player").transform.position[1] - GameObject.Find("Camera").transform.position[1] < -0.9f)
        {
            CreateBox(stuffToSay, sound, speed, 0, giveBackControl: true);
        }
        else
        {
            CreateBox(stuffToSay, sound, speed, 1, giveBackControl: true);
        }
    }

    public void CreateBox(string[] stuffToSay, string[] sound, int[] speed, string[] portraitNames)
    {
        if (GameObject.Find("Player").transform.position[1] - GameObject.Find("Camera").transform.position[1] < -0.9f)
        {
            CreateBox(stuffToSay, sound, speed, 0, giveBackControl: true, portraitNames);
        }
        else
        {
            CreateBox(stuffToSay, sound, speed, 1, giveBackControl: true, portraitNames);
        }
    }

    public void CreateBox(string[] stuffToSay, string sound, int speed, bool giveBackControl)
    {
        if (GameObject.Find("Player").transform.position[1] - GameObject.Find("Camera").transform.position[1] < -0.9f)
        {
            CreateBox(stuffToSay, sound, speed, 0, giveBackControl);
        }
        else
        {
            CreateBox(stuffToSay, sound, speed, 1, giveBackControl);
        }
    }

    public void CreateBox(string[] stuffToSay, bool giveBackControl)
    {
        CreateBox(stuffToSay, "snd_text", 0, giveBackControl);
    }

    public void CreateBox(string[] stuffToSay)
    {
        CreateBox(stuffToSay, giveBackControl: true);
    }

    public void CreateBox(TextSet textSet)
    {
        CreateBox(textSet.stuffToSay, textSet.sound, textSet.speed, textSet.location, textSet.giveBackControl, textSet.portraitNames, textSet.font);
    }

    public bool AtLastText()
    {
        if (lastString < currentString)
        {
            return true;
        }
        return false;
    }

    public bool IsPlaying()
    {
        return text.IsPlaying();
    }

    private void OnDestroy()
    {
        Destroy(menu);
        if (giveControl && (bool)Object.FindFirstObjectByType<GameManager>())
        {
            //Object.FindObjectOfType<GameManager>().EnablePlayerMovement();
        }
    }

    public void EnableChoice()
    {
        isControllable = false;
    }

    public void EnableSelectionAtEnd()
    {
        selectionEnabled = true;
    }

    public void DisableSelectionAtEnd()
    {
        selectionEnabled = false;
    }

    public bool IsSelectionEnabled()
    {
        return selectionEnabled;
    }

    public bool CanLoadSelection()
    {
        return canLoadSelection;
    }

    public GameObject GetUIBox()
    {
        return menu;
    }

    public int GetCurrentStringNum()
    {
        return currentString;
    }

    public void MakeUnskippable()
    {
        canSkip = false;
    }

    public void MakeSkippable()
    {
        canSkip = true;
    }

    public void Disable()
    {
        disabled = true;
    }

    public void Enable()
    {
        disabled = false;
    }

    public void ForceAdvanceCurrentLine()
    {
        forceAdvance = true;
    }

    private bool PortraitIsEmpty(int stringNum)
    {
        if (portraits == null)
        {
            return true;
        }
        if (portraits.Length == 0)
        {
            return true;
        }
        if (portraits.Length <= stringNum)
        {
            return true;
        }
        if (portraits[stringNum] != null)
        {
            return portraits[stringNum] == "";
        }
        return true;
    }

    public Image GetPortrait()
    {
        if ((bool)portrait)
        {
            return portrait.GetComponent<Image>();
        }
        return null;
    }

    public string GetCurrentSound()
    {
        int num = currentString - 1;
        if (num < 0)
        {
            return textSounds[0];
        }
        if (num >= textSounds.Length)
        {
            return textSounds[textSounds.Length - 1];
        }
        return textSounds[num];
    }

    public TextUT GetTextUT()
    {
        return text;
    }

    public void EnableGasterText()
    {
        text.EnableGasterEffect();
    }

    public void DisablePlayerControlOnDestroy()
    {
        giveControl = false;
    }

    public override void CancelControlReturn()
    {
        DisablePlayerControlOnDestroy();
    }

    public void AddRemark(int line, string[] text)
    {
        textRemarks.Add(line, new Queue<string>(text));
    }
}
