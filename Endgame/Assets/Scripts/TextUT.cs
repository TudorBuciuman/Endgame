using System.Collections;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.UI;
using UnityEngine.Windows;

public class TextUT : MonoBehaviour
{
    private string text;

    [SerializeField]
    private string currentText;

    private string remainingText;

    private Font font;

    [SerializeField]
    private Text txtObj;

    private bool doesExist;

    private Vector3 pos;

    private bool playing;

    private bool isControllable;

    private bool isColoring;

    private int colorPos;

    private int currentPos;

    private int finalPos;

    private int wait;

    private bool muted;

    private AudioSource as1;

    private AudioSource as2;

    private AudioClip sound;

    private string soundName;

    private int soundRandom;

    private bool enableGasterText;

    private int textSpeed;

    private float spacing;

    private Coroutine currentFadeOut;

    [SerializeField]
    private GameObject prefabObj;

    private Transform parent;

    private ButtonPrompts prompts;

    private int column;

    private int row;

    private AudioSource voiceSource;

    private Coroutine voiceSequenceRoutine;

    private bool voiceSequenceRunning;

    private void Awake()
    {
        as1 = base.gameObject.AddComponent<AudioSource>();
        as2 = base.gameObject.AddComponent<AudioSource>();
        text = "Sample Text";
        try
        {
            font = Util.PackManager().GetFont(Resources.Load<Font>("fonts/DTM-Mono"), "DTM-Mono");
        }
        catch
        {
            font = Resources.Load<Font>("fonts/DTM-Mono");
        }
        sound = Resources.Load<AudioClip>("sounds/snd_text");
        soundRandom = 0;
        playing = false;
        parent = base.transform;
        voiceSource = base.gameObject.AddComponent<AudioSource>();
    }

    private void Update()
    {
        for (int i = 0; i <= currentPos; i++)
        {
            Random.Range(0, 1);
        }
        if (!playing)
        {
            return;
        }
        if (soundRandom > 0)
        {
            sound = Resources.Load<AudioClip>("sounds/" + soundName + "_" + Random.Range(0, soundRandom));
        }
        if (!muted)
        {
            if (as1.volume == 0.5f)
            {
                as1.volume = 1f;
                as1.Stop();
            }
            if (as2.volume == 0.5f)
            {
                as2.volume = 1f;
                as2.Stop();
            }
        }
        if (wait > 0)
        {
            wait--;
        }
        else if (this.text.Length - currentPos >= 1)
        {
            int num = 1;
            string text = this.text.Substring(currentPos, 1);
            if (soundName.Contains("txtmtt"))
            {
                num = 3;
            }
            if (text != " " && text != "\t" && !muted)
            {
                PlayPitchedBlip();
            }
            for (int j = 0; j < num; j++)
            {
                if (!playing)
                {
                    break;
                }
                if (wait != 0)
                {
                    break;
                }
                text = this.text.Substring(currentPos, 1);
                TextRoutine(text);
                if (j == num - 1 && wait == textSpeed)
                {
                    wait += j;
                }
            }
        }
        else if (this.text.Length - currentPos <= 0)
        {
            playing = false;
        }
    }

    private void PlayPitchedBlip()
    {
        if (soundName.Contains("txtspam") || soundName.Contains("txtq_2"))
        {
            float pitch = 0.7f;
            if (soundName.Contains("txtq_2"))
            {
                pitch = Random.Range(0.95f, 1.05f);
            }
            if (currentPos % 2 == 0)
            {
                PlayBlipAudio(pitch);
            }
        }
        else
        {
            PlayBlipAudio();
        }
    }

    private void PlayBlipAudio(float pitch = 1f)
    {
        if (as1.isPlaying)
        {
            as1.volume = 0.5f;
            as2.Stop();
            as2.volume = 1f;
            as2.pitch = pitch;
            as2.clip = sound;
            as2.Play();
            return;
        }
        if (as2.isPlaying)
        {
            as2.volume = 0.5f;
            as1.Stop();
            as1.volume = 1f;
            as1.pitch = pitch;
            as1.clip = sound;
            as1.Play();
            return;
        }
        if (currentFadeOut != null)
        {
            StopCoroutine(currentFadeOut);
        }
        as1.Stop();
        as1.volume = 1f;
        as1.pitch = pitch;
        as1.clip = sound;
        as1.Play();
    }

    private void TextRoutine(string charc)
    {
        bool flag = false;
        /*
        if ((bool)prompts)
        {
            for (int i = 0; i < ButtonPrompts.validButtons.Length; i++)
            {
                if (charc == ButtonPrompts.GetButtonChar(ButtonPrompts.validButtons[i]))
                {
                    int num = ((txtObj.fontSize <= 20) ? 1 : 2);
                    float num2 = (-31f - (txtObj.lineSpacing - 1f) / 0.15f * 5f) / 2f * (float)num;
                    prompts.AddPrompt(txtObj.rectTransform, row * 8 * num, Mathf.RoundToInt((float)column * num2), ButtonPrompts.validButtons[i], num);
                    charc = " ";
                    flag = true;
                    break;
                }
            }
        }
        */
        if (charc != "\b")
        {
            currentText += charc;
        }
        if (flag)
        {
            currentText += " ";
        }
        row += ((!flag) ? 1 : 2);
        if (charc == "\n")
        {
            column++;
            row = 0;
        }
        if (finalPos - currentPos > 1)
        {
            if (charc == "\n" && text.Substring(currentPos + 1, 1) == " ")
            {
                currentText += " ";
                row++;
                currentPos++;
            }
            if (charc == "\b")
            {
                while (text.Substring(currentPos + 1, 1) == " ")
                {
                    currentText += " ";
                    row++;
                    currentPos++;
                }
            }
        }
        if (isColoring)
        {
            currentText = currentText.Remove(colorPos, 8);
            colorPos = currentText.Length;
            currentText += "</color>";
        }
        if (txtObj != null)
        {
            txtObj.text = currentText;
        }
        currentPos += charc.Length;
        wait = textSpeed + (charc.Length - 1);
        if (finalPos - currentPos > 6)
        {
            if (text.Substring(currentPos, 8) == "<color=#")
            {
                currentText += text.Substring(currentPos, 17);
                currentPos += 17;
                isColoring = true;
                colorPos = currentText.Length;
                currentText += "</color>";
            }
            else if (text.Substring(currentPos, 8) == "</color>")
            {
                currentPos += 8;
                isColoring = false;
            }
        }
        if (finalPos - currentPos > 2 && text.Substring(currentPos, 1) == "^")
        {
            currentPos++;
            wait = int.Parse(text.Substring(currentPos, 2));
            currentPos += 2;
        }
        if (currentPos > finalPos)
        {
            playing = false;
            wait = 0;
        }
    }

    public void StartText(string theText, Vector2 thePos)
    {
        row = 0;
        column = 0;
        if ((bool)prompts)
        {
            prompts.DeleteButtons();
            Object.Destroy(prompts);
        }
        theText = Util.Unescape(theText);
        theText = theText.Replace("^N", GameObject.Find("GameManager").GetComponent<GameManager>().GetPlayerName());
        if ((theText.Contains("^Z") || theText.Contains("^X") || theText.Contains("^C")) && UTInput.joystickIsActive)
        {
            prompts = base.gameObject.AddComponent<ButtonPrompts>();
        }
        /*
        theText = theText.Replace("^Z", UTInput.GetKeyOrButtonReplacement("Confirm"));
        theText = theText.Replace("^X", UTInput.GetKeyOrButtonReplacement("Cancel"));
        th

        eText = theText.Replace("^C", UTInput.GetKeyOrButtonReplacement("Menu"));
        currentFadeOut = StartCoroutine(AudioFadeOut.FadeOut(as1, 0.1f));
        */
        text = theText;
        pos = thePos;
        currentText = "";
        GameObject original = Resources.Load<GameObject>("ui/TextBase");
        if (enableGasterText)
        {
            original = Resources.Load<GameObject>("ui/TextBaseGaster");
        }
        prefabObj = Object.Instantiate(original, parent.position, Quaternion.identity);
        prefabObj.transform.SetParent(parent);
        prefabObj.transform.localPosition = pos;
        if (parent.gameObject.name == "BattleCanvas")
        {
            prefabObj.transform.localScale = new Vector2(1f, 1f);
        }
        txtObj = prefabObj.GetComponent<Text>();
        txtObj.GetComponent<LetterSpacing>().spacing = spacing;
        if (parent.gameObject.name.StartsWith("Speech"))
        {
            prefabObj.transform.localScale = new Vector2(1f, 1f);
            txtObj.fontSize = 13;
            txtObj.lineSpacing = 1.3f;
            txtObj.color = new Color(0f, 0f, 0f);
        }
        currentPos = 0;
        colorPos = 0;
        finalPos = text.Length - 1;
        playing = true;
        doesExist = true;
        if (text.Length >= 8 && text.Substring(0, 8) == "<color=#")
        {
            currentText += text.Substring(currentPos, 17);
            currentPos += 17;
            isColoring = true;
            colorPos = currentText.Length;
            currentText += "</color>";
        }
    }

    public void StartText(string theText, Vector2 thePos, string theSound)
    {
        if (voiceSource.isPlaying)
        {
            voiceSequenceRunning = false;
            voiceSource.Stop();
        }
        bool flag = DetermineUseVoiceSound(theSound);
        if (flag)
        {
            soundName = "";
        }
        else
        {
            soundName = theSound;
            if (soundName.Contains("txtmtt"))
            {
                soundRandom = 9;
                theSound = theSound + "_" + Random.Range(0, soundRandom);
            }
            else if (soundName.Contains("txtwd") || soundName.Contains("txtwdc"))
            {
                soundRandom = 7;
                theSound = theSound + "_" + Random.Range(0, soundRandom);
            }
            else if (soundName.Contains("txttem"))
            {
                soundRandom = 6;
                theSound = theSound + "_" + Random.Range(0, soundRandom);
            }
            else
            {
                soundRandom = 0;
            }
        }
        sound = Resources.Load<AudioClip>("sounds/" + theSound);
        muted = flag;
        StartText(theText, thePos);
    }

    public void StartText(string theText, Vector2 thePos, string theSound, int speed)
    {
        textSpeed = speed;
        StartText(theText, thePos, theSound);
    }

    public void StartText(string theText, Vector2 thePos, string theSound, int speed, string theFont)
    {
        bool num = theText.StartsWith("/WD");
        if (num)
        {
            theText = theText.Replace("/WD", "");
        }
        StartText(theText, thePos, theSound, speed);
        txtObj.font = Util.PackManager().GetFont(Resources.Load<Font>("fonts/" + theFont), theFont);
        switch (theFont)
        {
            case "sans":
            case "papyrus":
            case "wingdings":
                if (txtObj.fontSize > 20)
                {
                    txtObj.fontSize = 32;
                }
                else
                {
                    txtObj.fontSize = 16;
                }
                break;
            default:
                if (txtObj.fontSize > 20)
                {
                    txtObj.fontSize = 26;
                }
                else
                {
                    txtObj.fontSize = 13;
                }
                break;
        }
        if ((theFont == "DTM-Mono" || theFont == "speechbubble") && soundName.Contains("txtsans"))
        {
            txtObj.font = Resources.Load<Font>("fonts/sans");
            if (txtObj.fontSize > 20)
            {
                txtObj.fontSize = 32;
            }
            else
            {
                txtObj.fontSize = 16;
            }
        }
        if ((theFont == "DTM-Mono" || theFont == "speechbubble") && soundName.Contains("txtpap"))
        {
            txtObj.font = Resources.Load<Font>("fonts/papyrus");
            if (txtObj.fontSize > 20)
            {
                txtObj.fontSize = 32;
            }
            else
            {
                txtObj.fontSize = 16;
            }
        }
        if (num)
        {
            txtObj.font = Resources.Load<Font>("fonts/wingdings");
            if (txtObj.fontSize > 20)
            {
                txtObj.fontSize = 32;
            }
            else
            {
                txtObj.fontSize = 16;
            }
        }
        if (theFont == "DTM-Mono" && soundName.Contains("txtsat"))
        {
            txtObj.font = Resources.Load<Font>("fonts/saturn");
            if (txtObj.fontSize > 20)
            {
                txtObj.fontSize = 32;
            }
            else
            {
                txtObj.fontSize = 16;
            }
        }
    }

    public void DoText()
    {
        playing = true;
    }

    public void SkipText(bool sound = true)
    {
        if (playing)
        {
            if (sound)
            {
                PlayPitchedBlip();
            }
            playing = false;
            while (currentPos <= finalPos)
            {
                TextRoutine(text.Substring(currentPos, 1));
            }
            wait = 0;
        }
    }

    public bool IsPlaying()
    {
        return playing;
    }

    public void DestroyOldText()
    {
        SkipText(sound: false);
        Object.Destroy(prefabObj);
        doesExist = false;
        muted = false;
    }

    public bool Exists()
    {
        return doesExist;
    }

    public GameObject GetGameObject()
    {
        return prefabObj;
    }

    public Text GetText()
    {
        return txtObj;
    }

    public void SetParent(Transform trans)
    {
        parent = trans;
    }

    public void OnDestroy()
    {
        if (Exists())
        {
            DestroyOldText();
        }
    }

    public void SetLetterSpacing(float spacing)
    {
        this.spacing = spacing;
    }

    public void EnableGasterEffect()
    {
        enableGasterText = true;
    }

    private bool DetermineUseVoiceSound(string sound)
    {
        if (sound.StartsWith("#"))
        {
            if (sound.Contains("`"))
            {
                StartCoroutine(PlaySequentialVoiceLines(sound.Split('`')));
            }
            else
            {
                string path = "sounds/voice/" + sound.Trim('#');
                voiceSource.clip = Resources.Load<AudioClip>(path);
                voiceSource.Play();
            }
            return true;
        }
        return false;
    }

    private IEnumerator PlaySequentialVoiceLines(string[] voices)
    {
        voiceSequenceRunning = true;
        foreach (string text in voices)
        {
            string path = "sounds/voice/" + text.Trim('#');
            voiceSource.clip = Resources.Load<AudioClip>(path);
            voiceSource.Play();
            while (voiceSource.isPlaying && voiceSequenceRunning)
            {
                yield return null;
            }
        }
    }
}
