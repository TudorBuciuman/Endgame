using UnityEngine;

public class TextBubble : MonoBehaviour
{
    private string[] dialog;

    private Vector2 mainTextPos;

    private TextUT text;

    private int[] textSpeeds;

    private string[] textSounds;

    private string font;

    private int currentString;

    private int lastString;

    private bool firstString;

    private bool isControllable;

    private bool disabled;

    private bool sansFlag;

    private void Awake()
    {
        lastString = 0;
        firstString = false;
        isControllable = true;
        font = "speechbubble";
        base.gameObject.AddComponent<SwirlingText>();
    }

    private void Update()
    {
        if (lastString < 0)
        {
            return;
        }
        if (firstString)
        {
            text.StartText(dialog[currentString], mainTextPos, textSounds[currentString], textSpeeds[currentString], font);
            currentString++;
            firstString = false;
        }
        if (text.IsPlaying())
        {
            if ((UTInput.GetButton("X") || UTInput.GetButton("C")) && isControllable)
            {
                text.SkipText();
            }
        }
        else
        {
            if ((!UTInput.GetButtonDown("Z") && !UTInput.GetButton("C")) || disabled)
            {
                return;
            }
            bool flag = true;
            TextBubble[] array = Object.FindObjectsOfType<TextBubble>();
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].IsPlaying())
                {
                    flag = false;
                }
            }
            if (!flag)
            {
                return;
            }
            text.DestroyOldText();
            if (currentString <= lastString)
            {
                text.StartText(dialog[currentString], mainTextPos, textSounds[currentString], textSpeeds[currentString], font);
                currentString++;
                if ((UTInput.GetButton("X") || UTInput.GetButton("C")) && isControllable)
                {
                    text.SkipText();
                }
            }
            else if (CanMoveForward())
            {
                Object.Destroy(base.gameObject);
            }
        }
    }

    public void CreateBubble(string[] stuffToSay, int start, string sound, int speed, bool canSkip)
    {
        text = GetComponent<TextUT>();
        dialog = stuffToSay;
        currentString = start;
        lastString = dialog.Length - 1;
        firstString = true;
        isControllable = canSkip;
        mainTextPos = base.transform.Find("bubbletext").localPosition;
        textSounds = new string[dialog.Length];
        textSpeeds = new int[dialog.Length];
        for (int i = 0; i < dialog.Length; i++)
        {
            textSounds[i] = sound;
            textSpeeds[i] = speed;
        }
    }

    public void CreateBubble(string[] stuffToSay, int start, string sound, int speed, bool canSkip, string font)
    {
        this.font = font;
        CreateBubble(stuffToSay, start, sound, speed, canSkip);
    }

    public int GetCurrentStringNum()
    {
        return currentString;
    }

    public bool IsPlaying()
    {
        return text.IsPlaying();
    }

    public TextUT GetTextUT()
    {
        return text;
    }

    public bool CanMoveForward()
    {
        TextBubble[] array = Object.FindObjectsOfType<TextBubble>();
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].IsPlaying())
            {
                return false;
            }
        }
        return true;
    }

    public void Disable()
    {
        disabled = true;
    }

    public void Enable()
    {
        disabled = false;
    }

    public void ActivateSansFlag()
    {
        sansFlag = true;
    }
}
