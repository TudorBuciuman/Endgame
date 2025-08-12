using UnityEngine;
using UnityEngine.UI;

public class TextRemark : MonoBehaviour
{
    private Text text;

    private Image portrait;

    private bool playing;

    private int frames;

    private float xStart;

    private void Awake()
    {
        text = GetComponent<Text>();
        playing = false;
        frames = 0;
    }

    private void Update()
    {
        if (playing)
        {
            frames++;
            if (frames == 5)
            {
                playing = false;
            }
            float num = (float)frames / 5f;
            Color color = new Color(1f, 1f, 1f, num);
            text.color = color;
            if ((bool)portrait)
            {
                portrait.color = color;
            }
            base.transform.localPosition = new Vector3(Mathf.RoundToInt(Mathf.Lerp(xStart, xStart - 15f, num)), base.transform.localPosition.y);
        }
    }

    public void StartRemark(Vector3 position, string remk)
    {
        string text = "";
        string text2 = "br";
        string[] array = remk.Split('`');
        string str;
        if (array.Length == 3)
        {
            str = array[2];
            text = array[1];
            text2 = array[0];
        }
        else if (array.Length == 2)
        {
            str = array[1];
            text2 = array[0];
        }
        else
        {
            str = remk;
        }
        str = Util.Unescape(str);
        int num = 0;
        int num2 = 0;
        if (text2.Length == 2)
        {
            char c = text2[0];
            char c2 = text2[1];
            switch (c)
            {
                case 'b':
                    num2 = -1;
                    break;
                case 't':
                    num2 = 1;
                    break;
                case 'c':
                    num2 = 0;
                    break;
            }
            switch (c2)
            {
                case 'l':
                    num = -1;
                    break;
                case 'r':
                    num = 1;
                    break;
                case 'c':
                    num = 0;
                    break;
            }
        }
        position += new Vector3(num * 124, num2 * 32);
        this.text.text = str;
        if (text != "")
        {
            Sprite sprite = Resources.Load<Sprite>("overworld/npcs/portraits/spr_" + text + "_0");
            if (!sprite)
            {
                sprite = Resources.Load<Sprite>("overworld/npcs/portraits/spr_portrait_default_0");
            }
            portrait = new GameObject("PORTRAIT_" + text).AddComponent<Image>();
            portrait.sprite = sprite;
            portrait.GetComponent<RectTransform>().sizeDelta = new Vector2(sprite.rect.width / 24f, sprite.rect.height / 24f);
            portrait.color = new Color(1f, 1f, 1f, 0f);
            portrait.transform.SetParent(base.transform);
            portrait.transform.localPosition = new Vector3(-338f + sprite.rect.width % 2f, 28f + sprite.rect.height % 2f);
            if (text.Contains("sans"))
            {
                this.text.font = Resources.Load<Font>("fonts/sans");
                if (this.text.fontSize > 20)
                {
                    this.text.fontSize = 32;
                }
                else
                {
                    this.text.fontSize = 16;
                }
            }
            if (text.Contains("pap"))
            {
                this.text.font = Resources.Load<Font>("fonts/papyrus");
                if (this.text.fontSize > 20)
                {
                    this.text.fontSize = 32;
                }
                else
                {
                    this.text.fontSize = 16;
                }
            }
        }
        base.transform.localScale *= 0.5f;
        base.transform.localPosition = position;
        xStart = base.transform.localPosition.x;
        playing = true;
    }

    public bool CanAdvance()
    {
        return frames >= 4;
    }

    public void Skip()
    {
        playing = false;
        Color color = new Color(1f, 1f, 1f, 1f);
        text.color = color;
        if ((bool)portrait)
        {
            portrait.color = color;
        }
        base.transform.localPosition = new Vector3(xStart - 15f, base.transform.localPosition.y);
    }
}
