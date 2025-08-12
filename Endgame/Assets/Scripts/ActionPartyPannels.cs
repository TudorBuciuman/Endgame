using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ActionPartyPanels : UIComponent
{
    private GameManager gm;

    private bool[] isActive = new bool[4];

    private GameObject[] statPanels = new GameObject[4];

    private Image[] statBorders = new Image[4];

    private Image[][] roundBorders = new Image[4][];

    private RectTransform[] hpBars = new RectTransform[4];

    private Text[] hpText = new Text[4];

    private Text[] memberText = new Text[4];

    private int[] xPos = new int[4];

    private int[] hp = new int[4] { 20, 30, 20, 20 };

    private bool miniPartyMember;

    private bool activated;

    private bool raised;

    private int activeFrames;

    private bool ts;

    private bool bnp;

    private readonly Color[] bnpColors = new Color[3]
    {
        new Color32(117, 206, 251, byte.MaxValue),
        new Color32(197, 77, 170, byte.MaxValue),
        new Color32(byte.MaxValue, 219, 90, byte.MaxValue)
    };

    private void Awake()
    {
        gm = GameManager.instance;
        miniPartyMember = false;
        ts = false;
        //ts = Util.GameManager().GetFlagInt(94) == 1;
        bnp = SceneManager.GetActiveScene().buildIndex == 123;
        if (bnp)
        {
            Image[] componentsInChildren = base.transform.Find("Outlines").GetComponentsInChildren<Image>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                componentsInChildren[i].enabled = true;
            }
        }
        string[] array = new string[4] { "Kris", "Susie", "Noelle", "Mini" };
        for (int j = 0; j < 4; j++)
        {
            if (j < 3)
            {
                isActive[j] = j ==0;
            }
            else
            {
                isActive[j] = miniPartyMember;
            }
            statPanels[j] = base.transform.Find(array[j] + "Stats").gameObject;
            statBorders[j] = statPanels[j].GetComponent<Image>();
            statBorders[j].enabled = !ts;
            hpBars[j] = statBorders[j].transform.Find("Stats").Find("HPFG").GetComponent<RectTransform>();
            hpText[j] = statBorders[j].transform.Find("Stats").Find("HPTEXT").GetComponent<Text>();
            memberText[j] = statBorders[j].transform.Find("Stats").Find("name").GetComponent<Text>();
            if (isActive[j] && bnp)
            {
                statBorders[j].color = bnpColors[j];
            }
            roundBorders[j] = new Image[6];
            statBorders[j].color = Color.white;
            /*
            if ((int)gm.GetFlag(107) == 1 && j == 0)
            {
                if ((int)gm.GetFlag(108) == 1)
                {
                    statBorders[j].color = Color.white;//UIBackground.borderColors[(int)Util.GameManager().GetFlag(223)];
                }
                memberText[j].text = "Frisk";
                for (int k = 0; k < statBorders[j].transform.Find("Stats").childCount; k++)
                {
                    Transform child = statBorders[j].transform.Find("Stats").GetChild(k);
                    if (child.name != "name")
                    {
                        child.transform.localPosition += new Vector3(6f, 0f);
                    }
                }
            }
            */
            int num = 0;
            Image[] componentsInChildren = statBorders[j].transform.Find("roundedges").GetComponentsInChildren<Image>();
            foreach (Image image in componentsInChildren)
            {
                image.enabled = ts;
                roundBorders[j][num] = image;
                num++;
            }
            componentsInChildren = statBorders[j].transform.Find("roundcorners").GetComponentsInChildren<Image>();
            foreach (Image image2 in componentsInChildren)
            {
                image2.enabled = ts;
                roundBorders[j][num] = image2;
                num++;
            }
            componentsInChildren = statBorders[j].transform.Find("Stats").GetComponentsInChildren<Image>();
            foreach (Image image3 in componentsInChildren)
            {
                if (ts)
                {
                    if (!image3.enabled)
                    {
                        image3.enabled = true;
                    }
                    if (image3.gameObject.name == "HPFG")
                    {
                        image3.color = new Color(0f, 1f, 0f);
                    }
                }
                if (bnp)
                {
                    if (image3.gameObject.name == "HPFG")
                    {
                        image3.color = bnpColors[2];
                    }
                    else if (image3.gameObject.name == "HPBG")
                    {
                        image3.color = new Color32(142, 36, 63, byte.MaxValue);
                    }
                }
            }
            statPanels[j].SetActive(isActive[j]);
        }
        SetXPositions();
        UpdateHP(hp);
    }

    private void Update()
    {
        for (int i = 0; i < 4; i++)
        {
            if (isActive[i])
            {
                int num = (raised ? (-197) : (-279));
                if (i == 3 && raised)
                {
                    num += 29;
                }
                statPanels[i].transform.localPosition = Vector3.Lerp(statPanels[i].transform.localPosition, new Vector3(xPos[i], num), 0.5f);
            }
        }
        base.transform.Find("Outlines").localPosition = new Vector3(0f, statPanels[0].transform.localPosition.y);
        if (raised && !activated)
        {
            activeFrames++;
            if (activeFrames == 45)
            {
                Lower();
            }
        }
    }

    public void SetXPositions()
    {
        int num = NumOfActivePartyMembers();
        //int num2 = 2;

            xPos[0] = 0;
        for (int j = 0; j < 4; j++)
        {
            if (isActive[j])
            {
                statPanels[j].transform.localPosition = new Vector3(xPos[j], statPanels[j].transform.localPosition.y);
            }
        }
    }

    private void UpdateRoundedBorderColor(int i)
    {
        Image[] array = roundBorders[i];
        for (int j = 0; j < array.Length; j++)
        {
            array[j].color = statBorders[i].color;
        }
    }

    public void UpdateHP(int[] hp)
    {
        this.hp = hp;
        for (int i = 0; i < 4; i++)
        {
            if (!isActive[i])
            {
                continue;
            }
            int num = 0;
            int num2 = 0;
            if (i < 3)
            {
                num = hp[i];
                num2 = gm.GetMaxHP(i);
                if (i == 0 && miniPartyMember)
                {
                    //num -= gm.GetMiniMemberMaxHP();
                    if (num < 0)
                    {
                        num = 0;
                    }
                    //num2 -= gm.GetMiniMemberMaxHP();
                }
                statBorders[i].transform.Find("Stats").Find("HPBG").GetComponent<RectTransform>()
                    .sizeDelta = new Vector2((num2 >= 100) ? 25 : 45, 10f);
            }
            hpText[i].text = num.ToString((num2 >= 100) ? "D3" : "D2") + "/" + num2.ToString((num2 >= 100) ? "D3" : "D2");
            int num3 = Mathf.RoundToInt((float)num / (float)num2 * statBorders[i].transform.Find("Stats").Find("HPBG").GetComponent<RectTransform>()
                .sizeDelta.x);
            if (num3 < 1 && num > 0)
            {
                num3 = 1;
            }
            hpBars[i].sizeDelta = new Vector2(num3, 10f);
            if ((float)num < (float)num2 / 4f)
            {
                hpText[i].color = new Color(1f, 1f, 0f);
            }
            else
            {
                hpText[i].color = Color.white;
            }
            if (num == 0)
            {
                memberText[i].color = Color.grey;
                statBorders[i].color = Color.grey;
                hpText[i].color = Color.red;
                UpdateRoundedBorderColor(i);
            }
            else if (memberText[i].color == Color.grey)
            {
                memberText[i].color = Color.white;
                statBorders[i].color = Color.white;//PartyPanels.defaultColors[i];
                UpdateRoundedBorderColor(i);
            }
            if (SceneManager.GetActiveScene().buildIndex == 123)
            {
                if ((float)num / (float)num2 <= 0.1f)
                {
                    hpText[i].color = new Color(0.8784314f, 13f / 85f, 8f / 51f);
                }
                else if ((float)num / (float)num2 <= 0.3f)
                {
                    hpText[i].color = new Color(1f, 73f / 85f, 0.3529412f);
                }
                else if ((float)num / (float)num2 > 1f)
                {
                    hpText[i].color = new Color(0.1764706f, 0.6392157f, 0.18039216f);
                }
            }
        }
    }

    public int NumOfActivePartyMembers()
    {
        int num = 0;
        bool[] array = isActive;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i])
            {
                num++;
            }
        }
        return num;
    }

    public void SetActivated(bool activated)
    {
        this.activated = activated;
        activeFrames = 0;
    }

    public void UpdatePanels()
    {
        Awake();
    }

    public void Raise()
    {
        raised = true;
    }

    public void Lower()
    {
        raised = false;
        activeFrames = 0;
    }
}
