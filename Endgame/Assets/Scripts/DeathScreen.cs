using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathScreen : TranslatableBehaviour
{
    private int frames;

    private int stateText;

    private TextUT text;

    private bool toCredits;

    private int numDeaths;

    private bool done;
    private bool pressed;

    private int skipInputs;

    private int character;

    public override Dictionary<string, string[]> GetDefaultStrings()
    {
        Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>();
        dictionary.Add("REDACTED", new string[1] { "First time?" });
        return dictionary;
    }

    private void Awake()
    {
        SetStrings(GetDefaultStrings(), GetType());
        text = base.gameObject.GetComponent<TextUT>();
        text.SetLetterSpacing(15.3825f);
        frames = 0;
        done = false;
        character = Random.Range(0, 3);
        if ((int)Util.GameManager().GetSessionFlag(7) <= -1)
        {
            if (toCredits)
            {
                character = 4;
            }
        }
        else
        {
            character = (int)Util.GameManager().GetSessionFlag(7);
        }

        GetComponent<Image>().sprite = Util.PackManager().GetTranslatedSprite(GetComponent<Image>().sprite, "ui/spr_gameover");
    }

    private void Start()
    {
        FindFirstObjectByType<Fade>().transform.parent.position = Vector3.zero;
        GameObject obj = new GameObject("SOUL");
        obj.AddComponent<SOUL>();
        obj.GetComponent<SOUL>().CreateSOUL(SOUL.GetSOULColorByID(Util.GameManager().GetFlagInt(312)), monster: false, player: false);
        obj.transform.position = FindFirstObjectByType<GameManager>().GetSpawnPos();
        numDeaths = FindFirstObjectByType<GameManager>().GetNumDeaths();
    }

    private void Update()
    {
        if (!pressed)
        {
            if (!done)
            {
                if ((frames < 282))
                {
                    frames++;
                    if (frames == 19 && (bool)GameObject.Find("SOUL"))
                    {
                        GameObject.Find("SOUL").GetComponent<SOUL>().Break();
                    }
                    if (character != 4)
                    {
                        if (frames == 90)
                        {
                            GetComponent<AudioSource>().Play();
                        }
                        if (frames <= 140 && frames >= 90)
                        {
                            GetComponent<Image>().color = Color.Lerp(new Color(1f, 1f, 1f, 0f), Color.white, (float)(frames - 90) / 50f);
                        }
                    }
                    return;
                }
                else if (frames == 282)
                {
                    done = true;
                }
            }
            else if(!pressed)
            {
                if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return) || UTInput.GetButtonDown("Z"))
                {
                    pressed = true;
                    frames = 0;
                }
            }
        }
        else
        {
            GetComponent<AudioSource>().volume = Mathf.Lerp(1f, 0f, (float)frames / 100f);
            GetComponent<Image>().color = Color.Lerp(Color.white, new Color(1f, 1f, 1f, 0f), (float)frames / 100f);

            frames++;
            if (frames == 183)
            {
                FindFirstObjectByType<GameManager>().SpawnFromLastSave(true);
            }
        }
    }
}
