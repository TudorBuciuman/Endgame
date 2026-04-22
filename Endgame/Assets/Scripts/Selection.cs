using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Selection : UIComponent
{
    private GameObject[,] texts;

    private Vector2 index;

    private Vector2 maxIndex;

    private Vector2 prevIndex;

    private Vector3 mainPos;

    private Vector3 mainDif;

    private GameObject soul;

    private Vector3 soulDif;

    private bool isUsingSoul;

    private SelectableBehaviour origin;

    private int id;

    private bool playAudio;

    private bool isEnabled;

    private bool waitUntilUp;

    private bool axisIsDown;

    private bool xAxisIsDown;

    private bool yAxisIsDown;

    private bool wrap;

    public static Color[] selectionColors = new Color[12]
    {
        new Color(1f, 1f, 0f),
        new Color32(105, byte.MaxValue, 105, byte.MaxValue),
        new Color32(byte.MaxValue, 127, 142, byte.MaxValue),
        new Color(1f, 1f, 0f),
        new Color32(byte.MaxValue, 149, 79, byte.MaxValue),
        new Color32(82, 236, byte.MaxValue, byte.MaxValue),
        new Color32(byte.MaxValue, 109, 102, byte.MaxValue),
        new Color32(105, byte.MaxValue, 70, byte.MaxValue),
        new Color32(219, 122, 200, byte.MaxValue),
        new Color32(byte.MaxValue, 122, 202, byte.MaxValue),
        new Color(1f, 1f, 0f),
        new Color32(240, 240, 113, byte.MaxValue)
    };

    private void Awake()
    {
        playAudio = true;
        isUsingSoul = true;
        isEnabled = false;
        waitUntilUp = false;
        wrap = false;
        if (!Object.FindObjectOfType<BattleManager>())
        {
            if (UTInput.GetAxis("Horizontal") != 0f)
            {
                xAxisIsDown = true;
            }
            if (UTInput.GetAxis("Vertical") != 0f)
            {
                yAxisIsDown = true;
            }

            axisIsDown = xAxisIsDown || yAxisIsDown;
        }
    }

    private void Update()
    {
        if (isEnabled && !waitUntilUp)
        {
            if ((bool)soul && isUsingSoul && (bool)Object.FindObjectOfType<BattleManager>())
            {
                soul.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            }
            if (Mathf.Round(UTInput.GetAxisRaw("Horizontal")) != 0f || Mathf.Round(UTInput.GetAxisRaw("Vertical")) != 0f)
            {
                prevIndex = index;
                int num = 1;
                bool flag = false;
                bool flag2 = false;
                while (!xAxisIsDown && ((Mathf.Round(UTInput.GetAxisRaw("Horizontal")) == 1f && ((index[1] + (float)(Mathf.RoundToInt(UTInput.GetAxisRaw("Horizontal")) * (num - 1)) != maxIndex[1] && !wrap) || wrap)) || (Mathf.Round(UTInput.GetAxisRaw("Horizontal")) == -1f && ((index[1] + (float)(Mathf.RoundToInt(UTInput.GetAxisRaw("Horizontal")) * (num - 1)) != 0f && !wrap) || wrap))))
                {
                    flag = true;
                    if (wrap)
                    {
                        if (index[1] + (float)(Mathf.RoundToInt(UTInput.GetAxisRaw("Horizontal")) * num) > maxIndex[1])
                        {
                            index[1] = -1f;
                            num = 1;
                        }
                        else if (index[1] + (float)(Mathf.RoundToInt(UTInput.GetAxisRaw("Horizontal")) * num) < 0f)
                        {
                            index[1] = maxIndex[1] + 1f;
                            num = 1;
                        }
                    }
                    if (texts[(int)index[0], (int)index[1] + Mathf.RoundToInt(UTInput.GetAxisRaw("Horizontal")) * num].GetComponent<Text>().text != "")
                    {
                        index[1] += Mathf.Round(UTInput.GetAxisRaw("Horizontal")) * (float)num;
                        break;
                    }
                    num++;
                }
                num = 1;
                while (!yAxisIsDown && ((Mathf.Round(UTInput.GetAxisRaw("Vertical")) == -1f && ((index[0] - (float)(Mathf.RoundToInt(UTInput.GetAxisRaw("Vertical")) * (num - 1)) != maxIndex[0] && !wrap) || wrap)) || (Mathf.Round(UTInput.GetAxisRaw("Vertical")) == 1f && ((index[0] - (float)(Mathf.RoundToInt(UTInput.GetAxisRaw("Vertical")) * (num - 1)) != 0f && !wrap) || wrap))))
                {
                    flag2 = true;
                    if (wrap)
                    {
                        if (index[0] - (float)(Mathf.RoundToInt(UTInput.GetAxisRaw("Vertical")) * num) > maxIndex[0])
                        {
                            index[0] = -1f;
                            num = 1;
                        }
                        else if (index[0] - (float)(Mathf.RoundToInt(UTInput.GetAxisRaw("Vertical")) * num) < 0f)
                        {
                            index[0] = maxIndex[0] + 1f;
                            num = 1;
                        }
                    }
                    if (texts[(int)index[0] - Mathf.RoundToInt(UTInput.GetAxisRaw("Vertical")) * num, (int)index[1]].GetComponent<Text>().text != "")
                    {
                        index[0] -= Mathf.Round(UTInput.GetAxisRaw("Vertical")) * (float)num;
                        break;
                    }
                    num++;
                }
                if (index != prevIndex)
                {
                    if (playAudio)
                    {
                        texts[(int)index[0], (int)index[1]].GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("sounds/snd_menumove");
                        texts[(int)index[0], (int)index[1]].GetComponent<AudioSource>().Play();
                    }
                    if (!isUsingSoul)
                    {
                        texts[(int)prevIndex[0], (int)prevIndex[1]].GetComponent<Text>().color = new Color(1f, 1f, 1f);
                        //texts[(int)index[0], (int)index[1]].GetComponent<Text>().color = selectionColors[(int)Util.GameManager().GetFlag(223)];
                    }
                    else
                    {
                        float x = mainPos.x + soulDif.x + mainDif.x * index[1];
                        float y = mainPos.y + soulDif.y + mainDif.y * index[0];
                        soul.transform.localPosition = new Vector2(x, y);
                    }
                }
                if (flag)
                {
                    xAxisIsDown = true;
                }
                if (flag2)
                {
                    yAxisIsDown = true;
                }
            }
            if (UTInput.GetButtonDown("Z"))
            {
                axisIsDown = false;
                xAxisIsDown = false;
                yAxisIsDown = false;
                playAudio = true;
                if (playAudio)
                {
                    texts[(int)index[0], (int)index[1]].GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("sounds/snd_select");
                    texts[(int)index[0], (int)index[1]].GetComponent<AudioSource>().Play();
                }
                Decision();
            }
            if (Mathf.Round(UTInput.GetAxisRaw("Horizontal")) == 0f)
            {
                xAxisIsDown = false;
            }
            if (Mathf.Round(UTInput.GetAxisRaw("Vertical")) == 0f)
            {
                yAxisIsDown = false;
            }
            axisIsDown = xAxisIsDown || yAxisIsDown;
        }
        if (id == 100)
        {
            for (int i = 0; (float)i <= maxIndex[0]; i++)
            {
                for (int j = 0; (float)j <= maxIndex[1]; j++)
                {
                    texts[i, j].transform.localPosition = mainPos + new Vector3(mainDif[0] * (float)j + (float)Random.Range(-1, 1), mainDif[1] * (float)i + (float)Random.Range(-1, 1));
                }
            }
        }
        if (waitUntilUp && UTInput.GetButtonDown("Z"))
        {
            waitUntilUp = false;
        }
    }
    public void CreateSelections(string[,] sels, Vector2 firstPos, Vector2 difference, Vector2 soulDistFromPivot, string font, bool useSoul, bool makeSound, SelectableBehaviour originClass, int origId)
    {
        base.transform.localPosition = new Vector2(0f, 0f);
        mainPos = firstPos;
        mainDif = difference;
        maxIndex = new Vector2(sels.GetLength(0) - 1, sels.GetLength(1) - 1);
        texts = new GameObject[(int)maxIndex[0] + 1, (int)maxIndex[1] + 1];
        GameObject original = Resources.Load<GameObject>("ui/SelectionBase");
        origin = originClass;
        id = origId;
        for (int i = 0; (float)i <= maxIndex[0]; i++)
        {
            for (int j = 0; (float)j <= maxIndex[1]; j++)
            {
                texts[i, j] = Object.Instantiate(original, base.transform.position, Quaternion.identity);
                texts[i, j].name = id + "_" + sels[i, j];
                texts[i, j].transform.SetParent(base.transform);
                texts[i, j].transform.localPosition = mainPos + new Vector3(mainDif[0] * (float)j, mainDif[1] * (float)i);
                texts[i, j].GetComponent<Text>().text = sels[i, j];
                texts[i, j].GetComponent<Text>().font = Util.PackManager().GetFont(Resources.Load<Font>("fonts/" + font), font);
                
                if ((bool)Object.FindObjectOfType<BattleManager>())
                {
                    texts[i, j].transform.localScale = new Vector3(1f, 1f, 1f);
                }
            }
        }
        soulDif = soulDistFromPivot;
        index = Vector2.zero;
        isUsingSoul = true;
        
        if (isUsingSoul)
        {
            GameObject original2 = Resources.Load<GameObject>("ui/UISoul");
            soul = Object.Instantiate(original2, base.transform.position, Quaternion.identity);
            soul.transform.SetParent(base.transform);
            soul.transform.localPosition = mainPos + soulDif;
           // soul.GetComponent<Image>().color = SOUL.GetSOULColorByID(Util.GameManager().GetFlagInt(312));
           
            if ((bool)Object.FindObjectOfType<BattleManager>())
            {
                soul.transform.localScale = new Vector3(1f, 1f, 1f);
                soul.GetComponent<Image>().sprite = Resources.Load<Sprite>("battle/spr_soul");
                soul.GetComponent<RectTransform>().sizeDelta = new Vector2(16f, 16f);
                SOUL sOUL = null;
                SOUL[] array = Object.FindObjectsOfType<SOUL>();
                foreach (SOUL sOUL2 in array)
                {
                    if (sOUL2.IsPlayer())
                    {
                        sOUL = sOUL2;
                        break;
                    }
                }
                if ((bool)sOUL)
                {
                    soul.GetComponent<Image>().color = sOUL.GetSpriteColor();
                    if (sOUL.GetComponent<SpriteRenderer>().flipY)
                    {
                        soul.transform.localScale = new Vector3(1f, -1f, 1f);
                    }
                }
            }
            else if (SceneManager.GetActiveScene().buildIndex == 123)
            {
                soul.GetComponent<Image>().sprite = Resources.Load<Sprite>("overworld/spr_soul_ow_bnp");
            }
           
        }
    
        //else
       // {
            texts[0, 0].GetComponent<Text>().color = Color.white; //selectionColors[(int)Util.GameManager().GetFlag(223)];
        //}
        isEnabled = true;
        playAudio = makeSound;
    }
           
    public void Enable()
    {
        if (!isEnabled)
        {
            if (isUsingSoul)
            {
                soul.GetComponent<Image>().enabled = true;
            }
            else
            {
                texts[(int)index[0], (int)index[1]].GetComponent<Text>().color = Color.white;//selectionColors[(int)Util.GameManager().GetFlag(223)];
            }
            isEnabled = true;
            
        }
        texts[(int)index[0], (int)index[1]].GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("sounds/snd_menumove");
        texts[(int)index[0], (int)index[1]].GetComponent<AudioSource>().Play();
    }

    public void Disable()
    {
        if (isEnabled)
        {
            if (isUsingSoul)
            {
                soul.GetComponent<Image>().enabled = false;
            }
            else
            {
                texts[(int)index[0], (int)index[1]].GetComponent<Text>().color = new Color(1f, 1f, 1f);
            }
            isEnabled = false;
        }
    }

    public void Stick()
    {
        waitUntilUp = true;
    }

    public void CreateSelections(string[,] sels, Vector2 firstPos, Vector2 difference, SelectableBehaviour originClass, int origId)
    {
        CreateSelections(sels, firstPos, difference, new Vector2(-19f, 94f), "DTM-Mono", useSoul: true, makeSound: true, originClass, origId);
    }

    public void CreateSelections(string[,] sels, Vector2 firstPos, Vector2 difference, bool useSoul, SelectableBehaviour originClass, int origId)
    {
        CreateSelections(sels, firstPos, difference, new Vector2(-19f, 94f), "DTM-Mono", useSoul, makeSound: true, originClass, origId);
    }

    public void Reset()
    {
        for (int i = 0; (float)i <= maxIndex[0]; i++)
        {
            for (int j = 0; (float)j <= maxIndex[1]; j++)
            {
                Object.Destroy(texts[i, j].gameObject);
            }
        }
        texts = null;
        if (isUsingSoul)
        {
            Object.Destroy(soul);
        }
        isEnabled = false;
        id = 0;
    }
    
    public void Decision()
    {
        origin.MakeDecision(index, id);
    }

    public bool IsEnabled()
    {
        return isEnabled;
    }

    public Vector2 GetIndex()
    {
        return index;
    }
    
    public void ResetChoice()
    {
        index = Vector2.zero;
        if (isUsingSoul)
        {
            soul.transform.localPosition = mainPos + soulDif;
        }
        else
        {
            texts[0, 0].GetComponent<Text>().color = Color.white;//selectionColors[(int)Util.GameManager().GetFlag(223)];
        }
    }

    public void SetSelection(Vector2 newIndex)
    {
        index = newIndex;
        if (isUsingSoul)
        {
            soul.transform.localPosition = mainPos + soulDif + new Vector3(mainDif[0] * newIndex[1], mainDif[1] * newIndex[0]);
        }
        else
        {
            texts[(int)newIndex[0], (int)newIndex[1]].GetComponent<Text>().color = Color.white; //selectionColors[(int)Util.GameManager().GetFlag(223)];
        }
    }

    public void SetSelection(Vector2 newIndex, bool playSound)
    {
        SetSelection(newIndex);
        if (playSound)
        {
            texts[(int)index[0], (int)index[1]].GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("sounds/snd_menumove");
            texts[(int)index[0], (int)index[1]].GetComponent<AudioSource>().Play();
        }
    }

    public bool AxisDown()
    {
        return axisIsDown;
    }

    public void SetAxisDown(bool boo)
    {
        axisIsDown = boo;
        xAxisIsDown = boo;
        yAxisIsDown = boo;
    }

    public GameObject GetSOUL()
    {
        return soul;
    }

    public GameObject[,] GetSelectionTexts()
    {
        return texts;
    }

    public void ChangeSelectionText(string text, Vector2 index)
    {
        texts[(int)index[0], (int)index[1]].GetComponent<Text>().text = text;
    }

    public void SetWrap(bool wrap)
    {
        this.wrap = wrap;
    }
    
}
