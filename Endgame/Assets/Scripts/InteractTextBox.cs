using UnityEngine;

public class InteractTextBox : Interactable
{
    [SerializeField]
    public new bool enabled = true;

    [SerializeField]
    protected string[] lines = new string[1] { "* [NO_TEXT]" };

    [SerializeField]
    protected string[] sounds = new string[1] { "snd_text" };

    [SerializeField]
    private int[] speed = new int[1];

    [SerializeField]
    protected string[] portraits;

    [SerializeField]
    protected Remark[] remarks;

    [SerializeField]
    protected bool secondaryLines;

    [SerializeField]
    protected string[] lines2 = new string[1] { "* [REDACTED]" };

    [SerializeField]
    protected string[] sounds2 = new string[1] { "snd_text" };

    [SerializeField]
    protected int[] speed2 = new int[1];

    [SerializeField]
    protected string[] portraits2;

    [SerializeField]
    protected Remark[] remarks2;

    protected bool talkedToBefore;

    [SerializeField]
    private int vanishAtFlag = -1;

    [SerializeField]
    private int vanishAtCount;

    [SerializeField]
    private int triggerFlag = -1;

    [SerializeField]
    private int forceSecondaryAtFlag = -1;

    [SerializeField]
    protected Sprite[] talkSprites;

    [SerializeField]
    protected int talkFramerate = 6;

    protected int talkFrames;

    protected string talkName = "Talk";

    protected virtual void Awake()
    {
        
        if (forceSecondaryAtFlag > -1 && (int)Util.GameManager().GetFlag(forceSecondaryAtFlag) == 1)
        {
            talkedToBefore = true;
        }
        if (vanishAtFlag > -1 && (int)FindFirstObjectByType<GameManager>().GetFlag(vanishAtFlag) >= vanishAtCount)
        {
            Object.Destroy(base.gameObject);
        }
    }

    protected virtual void Update()
    {
        if ((bool)txt)
        {
            if ((bool)GetComponent<Animator>() && GetComponent<Animator>().enabled)
            {
                if (txt.IsPlaying())
                {
                    talkFrames++;
                    if (talkFrames == 1 && GetComponent<Animator>().HasState(0, Animator.StringToHash(talkName)))
                    {
                        GetComponent<Animator>().Play(talkName, 0, 0f);
                    }
                    return;
                }
                if (talkFrames > 0)
                {
                    string stateName = (GetComponent<Animator>().HasState(0, Animator.StringToHash("Idle")) ? "Idle" : "idle");
                    GetComponent<Animator>().Play(stateName, 0, 0f);
                }
                talkFrames = 0;
            }
            else if (txt.IsPlaying() && talkSprites != null)
            {
                talkFrames++;
                if (talkSprites.Length != 0)
                {
                    GetComponent<SpriteRenderer>().sprite = talkSprites[talkFrames / talkFramerate % talkSprites.Length];
                }
            }
            else if (talkSprites != null)
            {
                talkFrames = 0;
                if (talkSprites.Length != 0)
                {
                    GetComponent<SpriteRenderer>().sprite = talkSprites[0];
                }
            }
        }
        else if (talkFrames > 0)
        {
            talkFrames = 0;
            if (talkSprites != null && talkSprites.Length != 0)
            {
                GetComponent<SpriteRenderer>().sprite = talkSprites[0];
            }
            if ((bool)GetComponent<Animator>() && GetComponent<Animator>().enabled)
            {
                string stateName2 = (GetComponent<Animator>().HasState(0, Animator.StringToHash("Idle")) ? "Idle" : "idle");
                GetComponent<Animator>().Play(stateName2, 0, 0f);
            }
        }
    }

    public override void DoInteract()
    {
        if ((bool)txt || !enabled || !GameManager.instance.canInteract)
        {
            return;
        }
        /*
        if ((int)Util.GameManager().GetSessionFlag(6) == 1)
        {
            txt = new GameObject("InteractTextBox", typeof(TextBox)).GetComponent<TextBox>();
            txt.CreateBox(new string[1] { "* ..." }, giveBackControl: true);
            Object.FindObjectOfType<GameManager>().DisablePlayerMovement(deactivatePartyMembers: false);
            return;
        }
        */
        GameManager.instance.canMove = false;
        GameManager.instance.canInteract = false;

        if (talkedToBefore && secondaryLines)
        {
            CreateTextBox(lines2, sounds2, speed2, giveBackControl: true, portraits2, remarks2);
        }
        else
        {
            CreateTextBox(lines, sounds, speed, giveBackControl: true, portraits, remarks);
        }
        talkedToBefore = true;

        if (triggerFlag > -1 && Util.GameManager().GetFlagInt(triggerFlag) == 0)
        {
            Util.GameManager().SetFlag(triggerFlag, 1);
        }
    }
    public override int GetEventData()
    {
        return -1;
    }

    public override void MakeDecision(Vector2 index, int id)
    {
        Debug.LogError("Tried to make decision in simple textbox interactable");
    }

    public void ModifyContents(string[] lines, string[] sounds, int[] speed, string[] portraits)
    {
        this.lines = lines;
        this.sounds = sounds;
        this.speed = speed;
        this.portraits = portraits;
        talkedToBefore = false;
    }

    public void ModifySecondaryContents(string[] lines, string[] sounds, int[] speed, string[] portraits)
    {
        lines2 = lines;
        sounds2 = sounds;
        speed2 = speed;
        portraits2 = portraits;
    }

    public void ShrinkLines(int size)
    {
        string[] array = new string[size];
        for (int i = 0; i < size; i++)
        {
            array[i] = lines[i];
        }
        lines = array;
    }

    public void ShrinkSecondaryLines(int size)
    {
        string[] array = new string[size];
        for (int i = 0; i < size; i++)
        {
            array[i] = lines2[i];
        }
        lines2 = array;
    }

    public override void SetTalkable(TextBox txt)
    {
        if (!txt)
        {
            talkFrames = 0;
        }
        base.SetTalkable(txt);
    }

    public void DisableSecondaryLines()
    {
        secondaryLines = false;
    }

    public void EnableSecondaryLines()
    {
        secondaryLines = true;
    }

    public void ForceTalkedToBefore()
    {
        talkedToBefore = true;
    }

    public string[] GetLines()
    {
        return lines;
    }

    public string[] GetSounds()
    {
        return sounds;
    }

    public string[] GetPortraits()
    {
        return portraits;
    }
}
