using UnityEngine;

public class BattleButton : MonoBehaviour
{
    [SerializeField]
    private string type;

    private bool isSelected;

    private string suffix = "";

    private Color color = new Color32(byte.MaxValue, 127, 39, byte.MaxValue);

    private Color selColor = new Color(1f, 1f, 0f);

    public static Color[] buttonColors = new Color[12]
    {
        new Color32(byte.MaxValue, 127, 39, byte.MaxValue),
        new Color32(0, 216, 140, byte.MaxValue),
        new Color32(byte.MaxValue, 0, 89, byte.MaxValue),
        new Color32(byte.MaxValue, 127, 39, byte.MaxValue),
        new Color32(206, 102, 33, byte.MaxValue),
        new Color32(82, 116, byte.MaxValue, byte.MaxValue),
        new Color32(byte.MaxValue, 51, 73, byte.MaxValue),
        new Color32(74, 175, 74, byte.MaxValue),
        new Color32(143, 146, 222, byte.MaxValue),
        Color.white,
        new Color32(84, 84, 124, byte.MaxValue),
        new Color32(165, 38, 38, byte.MaxValue)
    };

    private void Awake()
    {
        if ((int)Object.FindObjectOfType<GameManager>().GetFlag(94) == 1)
        {
            suffix = "_ts";
            isSelected = true;
            color = new Color32(170, 114, 190, byte.MaxValue);
            selColor = new Color32(211, 142, 232, byte.MaxValue);
        }
        int num = (int)Util.GameManager().GetFlag(223);
        if (num > 0)
        {
            color = buttonColors[num];
            selColor = Selection.selectionColors[num];
        }
        isSelected = false;
        UpdateSprite();
    }

    public void Select(bool boo)
    {
        if (boo && !isSelected)
        {
            Object.FindObjectOfType<BattleManager>().ButtonSFX();
            isSelected = true;
        }
        else if (!boo && isSelected)
        {
            isSelected = false;
        }
        UpdateSprite();
    }

    public void ChangeButtonType(string type)
    {
        if (this.type != type)
        {
            this.type = type;
            UpdateSprite();
        }
    }

    public void ChangeButtonSuffix(string suffix)
    {
        if (this.suffix != suffix)
        {
            this.suffix = suffix;
            UpdateSprite();
        }
    }

    public string GetButtonType()
    {
        return type;
    }

    private void UpdateSprite()
    {
        if (isSelected)
        {
            string text = "battle/spr_" + type + "bt_1" + suffix;
            GetComponent<SpriteRenderer>().sprite = Util.PackManager().GetTranslatedSprite(Resources.Load<Sprite>(text), text);
            GetComponent<SpriteRenderer>().color = selColor;
        }
        else
        {
            string text2 = "battle/spr_" + type + "bt_0" + suffix;
            GetComponent<SpriteRenderer>().sprite = Util.PackManager().GetTranslatedSprite(Resources.Load<Sprite>(text2), text2);
            GetComponent<SpriteRenderer>().color = color;
        }
    }
}
