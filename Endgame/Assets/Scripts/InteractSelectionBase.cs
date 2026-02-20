using UnityEngine;

public class InteractSelectionBase : Interactable
{
    [SerializeField]
    public new bool enabled = true;

    [SerializeField]
    protected string[] lines = new string[1] { "* [NO_TEXT]" };

    [SerializeField]
    protected string[] sounds = new string[1] { "snd_text" };

    [SerializeField]
    protected int[] speed = new int[1];

    [SerializeField]
    protected string[] portraits;

    [SerializeField]
    protected Remark[] remarks;

    [SerializeField]
    protected string left = "";

    [SerializeField]
    protected Vector2 leftOffset = Vector2.zero;

    [SerializeField]
    protected string right = "";

    [SerializeField]
    protected Vector2 rightOffset = Vector2.zero;

    [SerializeField]
    protected string up = "";

    [SerializeField]
    protected Vector2 upOffset = Vector2.zero;

    [SerializeField]
    protected string down = "";

    [SerializeField]
    protected Vector2 downOffset = Vector2.zero;

    [SerializeField]
    protected Vector2 centerOffset = Vector2.zero;

    [SerializeField]
    private Sprite[] talkSprites;

    [SerializeField]
    private int talkFramerate = 6;

    private int talkFrames;

    protected bool selectActivated;

    protected int selectID;

    protected virtual void Update()
    {
        if ((bool)txt)
        {
            if (txt.IsPlaying() && talkSprites != null)
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
            HandleTextExist();
        }
        else if (talkFrames > 0 && talkSprites != null)
        {
            talkFrames = 0;
            if (talkSprites.Length != 0)
            {
                GetComponent<SpriteRenderer>().sprite = talkSprites[0];
            }
        }
    }

    public override void DoInteract()
    {
        if (!txt && enabled)
        {
            CreateTextBox(lines, sounds, speed, giveBackControl: false, portraits, remarks);
            GameManager.instance.canMove = false;
            GameManager.instance.canInteract = false;
            Debug.Log(2);
            txt.EnableSelectionAtEnd();
        }
    }

    protected virtual void HandleTextExist()
    {
        if (txt.CanLoadSelection() && !selectActivated)
        {
            selectActivated = true;
            DeltaSelection component = Object.Instantiate(Resources.Load<GameObject>("ui/DeltaSelection"), Vector3.zero, Quaternion.identity, txt.GetUIBox().transform).GetComponent<DeltaSelection>();
            component.SetupChoice(Vector2.left, left, leftOffset);
            component.SetupChoice(Vector2.right, right, rightOffset);
            component.SetupChoice(Vector2.up, up, upOffset);
            component.SetupChoice(Vector2.down, down, downOffset);
            component.Activate(this, selectID, txt.gameObject);
        }
    }

    public override int GetEventData()
    {
        return -1;
    }

    public override void MakeDecision(Vector2 index, int id)
    {
        GameManager.instance.canMove = true;
        GameManager.instance.canInteract = true;
        selectActivated = false;
        MonoBehaviour.print(string.Concat(index, " ", id));
    }

    public void ModifyContents(string[] lines, string[] sounds, int[] speed, string[] portraits)
    {
        this.lines = lines;
        this.sounds = sounds;
        this.speed = speed;
        this.portraits = portraits;
    }
}
