using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class MusicChooser : MonoBehaviour
{
    public static readonly int FRANKNESS_ID = 14;

    private MusicPlayer mus;

    private AudioSource sfx;

    private bool axisHold = true;

    private bool showdown;

    public static int musicID = 0;

    private void Awake()
    {
        sfx = GetComponent<AudioSource>();
        mus = base.gameObject.AddComponent<MusicPlayer>();
        GetComponent<Image>().color = UIBackground.borderColors[Util.GameManager().GetFlagInt(223)];
        UpdateListing();
    }

    private void Start()
    {
        base.transform.Find("SOUL").GetComponent<Image>().color = Color.red;//SOUL.GetSOULColorByID(Util.GameManager().GetFlagInt(312));
    }

    private void Update()
    {
        if (UTInput.GetAxis("Horizontal") != 0f && !axisHold)
        {
            sfx.Play();
            axisHold = true;
            string text = ((UTInput.GetAxis("Horizontal") > 0f) ? "Right" : "Left");
            base.transform.Find(text + "Arrow").GetComponent<Text>().color = new Color(1f, 1f, 0f);
            musicID += Mathf.RoundToInt(UTInput.GetAxis("Horizontal"));
            if (musicID < 0)
            {
                //musicID = UnoGameManager.GetUnoMusicArray().Length - 1;
            }
            //else if (UnoGameManager.GetUnoMusic(musicID)[0].Key == "music/mus_date")
            //{
                musicID = 0;
            //}
            UpdateListing();
        }
        else if (UTInput.GetAxis("Horizontal") == 0f && axisHold)
        {
            axisHold = false;
            base.transform.Find("LeftArrow").GetComponent<Text>().color = Color.white;
            base.transform.Find("RightArrow").GetComponent<Text>().color = Color.white;
        }
        if (UTInput.GetButtonDown("Z"))
        {
            PlayNewMusic(0);
        }
        else if (UTInput.GetButtonDown("X") && !showdown)
        {
            PlayNewMusic(1);
        }
        else if (UTInput.GetButtonDown("C"))
        {
            Object.Destroy(base.gameObject);
        }
    }

    private void UpdateListing()
    {
        base.transform.Find("MusicID").GetComponent<Text>().text = "MusicID: " + musicID;
        {
            showdown = true;
            base.transform.Find("NormalThemeHeader").GetComponent<Text>().enabled = false;
            base.transform.Find("NormalTheme").GetComponent<Text>().enabled = false;
            base.transform.Find("NormalThemePlay").GetComponent<Text>().enabled = false;
            base.transform.Find("NormalThemeDescription").GetComponent<Text>().enabled = false;
            base.transform.Find("TenseThemeHeader").GetComponent<Text>().enabled = false;
            base.transform.Find("TenseTheme").GetComponent<Text>().enabled = false;
            base.transform.Find("TenseThemePlay").GetComponent<Text>().enabled = false;
            base.transform.Find("TenseThemeDescription").GetComponent<Text>().enabled = false;
            //string[] array3 = Localizer.GetText(UnoGameManager.GetUnoMusic(musicID)[0].Key).Split('`');
            base.transform.Find("1v1ThemeHeader").GetComponent<Text>().enabled = true;
            base.transform.Find("1v1Theme").GetComponent<Text>().enabled = true;
            //base.transform.Find("1v1Theme").GetComponent<Text>().text = array3[0];
            base.transform.Find("1v1ThemePlay").GetComponent<Text>().enabled = true;
            base.transform.Find("1v1ThemePlay").GetComponent<Text>().color = Color.white;
            base.transform.Find("1v1ThemeDescription").GetComponent<Text>().enabled = true;
            //base.transform.Find("1v1ThemeDescription").GetComponent<Text>().text = Util.BattleHUDFontFix(array3[1]);
            base.transform.Find("1v1PapWarning").GetComponent<Text>().enabled = musicID == FRANKNESS_ID;
        }
        string[] array4 = new string[4] { "press {0} to preview normal theme", "press {0} to preview tense theme", "press {0} to preview theme", "press {0} to exit" };
        string[] array5 = new string[4] { "NormalThemePlay", "TenseThemePlay", "1v1ThemePlay", "Exit" };
        string[] array6 = new string[4] { "Confirm", "Cancel", "Confirm", "Menu" };
        bool joystickIsActive = UTInput.joystickIsActive;
        /*
        for (int i = 0; i < 4; i++)
        {
            Transform transform = base.transform.Find(array5[i]);
            transform.GetComponent<Text>().text = string.Format(array4[i], joystickIsActive ? " " : $"[{UTInput.GetKeyName(array6[i])}]");
            if (joystickIsActive && transform.GetComponent<Text>().enabled)
            {
                transform.GetComponentInChildren<Image>().enabled = true;
                for (int j = 0; j < ButtonPrompts.validButtons.Length; j++)
                {
                    if (UTInput.GetKeyOrButtonReplacement(array6[i]) == ButtonPrompts.GetButtonChar(ButtonPrompts.validButtons[j]))
                    {
                        transform.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("ui/buttons/" + ButtonPrompts.GetButtonGraphic(ButtonPrompts.validButtons[j]));
                        break;
                    }
                }
            }
            else
            {
                transform.GetComponentInChildren<Image>().enabled = false;
            }
        }*/
    }

    private void PlayNewMusic(int tense)
    {
        bool flag = false;
        if (mus.IsPlaying())
        {
            /*
            if (mus.CurrentMusic() == UnoGameManager.GetUnoMusic(musicID)[tense].Key.Replace("_intro", ""))
            {
                mus.Stop();
                if (showdown)
                {
                    base.transform.Find("1v1ThemePlay").GetComponent<Text>().color = Color.white;
                }
                else if (tense == 1)
                {
                    base.transform.Find("TenseThemePlay").GetComponent<Text>().color = Color.white;
                }
                else
                {
                    base.transform.Find("NormalThemePlay").GetComponent<Text>().color = Color.white;
                }
            }
            else
            {
                mus.Stop();
                if (tense == 1 && !showdown)
                {
                    base.transform.Find("NormalThemePlay").GetComponent<Text>().color = Color.white;
                }
                else if (tense == 0 && !showdown)
                {
                    base.transform.Find("TenseThemePlay").GetComponent<Text>().color = Color.white;
                }
                flag = true;
            }
        }
        else
        {
            flag = true;
        }
        if (flag)
        {
            string key = UnoGameManager.GetUnoMusic(musicID)[tense].Key;
            bool flag2 = key.EndsWith("_intro");
            mus.ChangeMusic(flag2 ? key.Replace("_intro", "") : key, flag2, playImmediately: true);
            mus.GetSource().pitch = UnoGameManager.GetUnoMusic(musicID)[tense].Value;
            if (showdown)
            {
                base.transform.Find("1v1ThemePlay").GetComponent<Text>().color = new Color(1f, 1f, 0f);
            }
            else if (tense == 1)
            {
                base.transform.Find("TenseThemePlay").GetComponent<Text>().color = new Color(1f, 1f, 0f);
            }
            else
            {
                base.transform.Find("NormalThemePlay").GetComponent<Text>().color = new Color(1f, 1f, 0f);
            }*/
        }
    }
}
