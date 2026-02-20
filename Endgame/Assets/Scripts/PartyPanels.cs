using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class PartyPanels : MonoBehaviour
{
    private GameManager gm;

    private bool[] isActive = new bool[4];

    private GameObject[] statPanels = new GameObject[4];

    private Image[] statBorders = new Image[4];

    private Image[][] roundBorders = new Image[4][];

    private RectTransform[] hpBars = new RectTransform[4];

    private Text[] hpText = new Text[4];

    private Text[] memberText = new Text[4];

    private Image[] memberSprite = new Image[4];

    private int[] xPos = new int[4];

    private bool[] raiseHead = new bool[3] { true, false, false };

    private bool defense;

    private int hp = 20;

    private int[] revivalTurns = new int[3];

    private bool[] targets = new bool[3];

    private bool hpCalibrated;

    private bool[] defending = new bool[4];

    private bool miniPartyMember;

    private bool miniPartyMemberDisabled;

    public static readonly Color[] defaultColors = new Color[7]
    {
        new Color(0f, 1f, 1f),
        new Color(1f, 0f, 1f),
        new Color(1f, 1f, 0f),
        Color.red,
        Color.green,
        Color.blue,
        new Color(0f, 1f, 1f)
    };

    private void Awake()
    {
        gm = FindFirstObjectByType<GameManager>();
        for (int i = 0; i < 1; i++)
        {
            hp = gm.GetHP();
            isActive[i] = true;
            statPanels[i] = base.transform.Find("BattleStats").gameObject;
            statBorders[i] = statPanels[i].GetComponent<Image>();
            hpBars[0] = statBorders[i].transform.Find("Stats").Find("HPFG").GetComponent<RectTransform>();
            hpBars[1] = statBorders[i].transform.Find("Stats").Find("HPBG").GetComponent<RectTransform>();
            hpText[i] = statBorders[i].transform.Find("Stats").Find("HPTEXT").GetComponent<Text>();
            statBorders[i].transform.Find("Stats").Find("LV").GetComponent<Text>().text = "                  LV  "+gm.GetLV().ToString();
            int level = gm.GetLV();
            float textScale = 1f + (level - 1) * 0.05f;
            Vector2 textOffset = new Vector2((level - 1) * 3f, 0f);
            Vector2 textOffsetText = new Vector2(Mathf.FloorToInt((level - 1) * 0.5f), 0f);
            Vector2 min = hpBars[0].offsetMin;
            Vector2 max = hpBars[0].offsetMax;
            max.x = -82f + textOffset.x;
            
            RectTransform hpTextRect = hpText[0].GetComponent<RectTransform>();
            hpTextRect.anchoredPosition += textOffsetText;
            for (int j = 0; j <=1; j++)
            {
                RectTransform barRect = hpBars[j];
                barRect.offsetMin = min;
                barRect.offsetMax = max;
            }
            hpText[0].rectTransform.anchoredPosition += textOffset;
        }
    }

    public void SetXPositions()
    {
        int num = NumOfActivePartyMembers();
            xPos[0] = 0;
        for (int j = 0; j < 1; j++)
        {
            if (isActive[j])
            {
                statPanels[j].transform.localPosition = new Vector3(xPos[j], statPanels[j].transform.localPosition.y);
                //memberSprite[j].transform.localPosition = new Vector3(xPos[j], memberSprite[j].transform.localPosition.y);
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
    public void UpdateHP(int hp)
    {
        int level = gm.GetLV(); 
        float textScale = 1f + (level - 1) * 0.05f; 
        Vector2 textOffset = new Vector2((level - 1) * 3f, 0f); 
        int maxHP = gm.GetMaxHP();
        int currentHP = hp;

        RectTransform hpBG = statBorders[0].transform.Find("Stats/HPFG").GetComponent<RectTransform>();
        RectTransform barRect = hpBars[0];

        float barRatio = Mathf.Clamp01((float)currentHP / maxHP);

        float fullRight = -82f + textOffset.x;  
        float emptyRight = -145f;                

        Vector2 min = barRect.offsetMin;
        Vector2 max = barRect.offsetMax;

        max.x = Mathf.Lerp(emptyRight, fullRight, barRatio);

        barRect.offsetMin = min;
        barRect.offsetMax = max;

        string format = (maxHP >= 100) ? "D3" : "D2";
        hpText[0].text = currentHP.ToString(format) + "/" + maxHP.ToString(format);
        this.hp = hp;
        hpCalibrated = true;
    }
    public void UpdateLV()
    {
        statBorders[0].transform.Find("Stats").Find("LV").GetComponent<Text>().text = "                  LV  " + gm.GetLV().ToString();
        UpdateHP(gm.GetHP());
    }
    public Transform GetStatPanel(int i)
    {
        return statPanels[i].transform;
    }

    public Color GetDefaultColor(int i)
    {
        return defaultColors[i];
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
        if (miniPartyMember)
        {
            num--;
        }
        return num;
    }

    public bool[] GetTargettedMembers()
    {
        return targets;
    }

    public bool IsDefending(int partyMember)
    {
        return defending[partyMember];
    }

    public void Reinitialize()
    {
        Awake();
    }

    public void SetXOffset(int i, int x)
    {
        xPos[i] = x;
    }
}
