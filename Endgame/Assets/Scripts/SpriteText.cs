using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(CanvasRenderer))]
public class SpriteText : Graphic
{
    [SerializeField]
    private string text = "";

    [SerializeField]
    private string spritePath = "sprites/ui/spr_font_small";

    [SerializeField]
    private int characterSpacing;

    [SerializeField]
    private int spaceSize = 8;

    [SerializeField]
    private string customCharacterSet = "";

    [SerializeField]
    private Vector2 inset = new Vector2(4f, -4f);

    private readonly string CHARACTER_SET = "abcdefghijklmnopqrstuvwxyz0123456789!?-=.,#*/[]:";

    private Sprite[] spriteSet;

    private Texture texture;

    public string Text
    {
        get
        {
            return text;
        }
        set
        {
            if (!(text == value.ToLower()))
            {
                text = value.ToLower();
                SetVerticesDirty();
            }
        }
    }

    public string SpritePath
    {
        get
        {
            return spritePath;
        }
        set
        {
            if (!(spritePath == value))
            {
                spritePath = value;
                spriteSet = null;
                SetVerticesDirty();
            }
        }
    }

    public int CharacterSpacing
    {
        get
        {
            return characterSpacing;
        }
        set
        {
            if (characterSpacing != value)
            {
                characterSpacing = value;
                SetVerticesDirty();
            }
        }
    }

    public int SpaceSize
    {
        get
        {
            return spaceSize;
        }
        set
        {
            if (spaceSize != value)
            {
                spaceSize = value;
                SetVerticesDirty();
            }
        }
    }

    public string CustomCharacterSet
    {
        get
        {
            return customCharacterSet;
        }
        set
        {
            if (!(customCharacterSet == value))
            {
                customCharacterSet = value;
                SetVerticesDirty();
            }
        }
    }

    public Vector2 Inset
    {
        get
        {
            return inset;
        }
        set
        {
            if (!(inset == value))
            {
                inset = value;
                SetVerticesDirty();
            }
        }
    }

    public override Texture mainTexture => texture;

    private string GetCharacterSet()
    {
        if (customCharacterSet != "")
        {
            return customCharacterSet;
        }
        return CHARACTER_SET;
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        base.OnPopulateMesh(vh);
        if (spriteSet == null)
        {
            spriteSet = Resources.LoadAll<Sprite>(spritePath);
        }
        vh.Clear();
        int num = 0;
        string characterSet = GetCharacterSet();
        if (spriteSet.Length != characterSet.Length)
        {
            return;
        }
        texture = spriteSet[0].texture;
        for (int i = 0; i < text.Length; i++)
        {
            int num2 = characterSet.IndexOf(text[i]);
            if (num2 > -1 && num2 < spriteSet.Length)
            {
                DrawLetter(vh, num2, new Vector3(num, 0f));
                num += (int)spriteSet[num2].rect.width;
            }
            else
            {
                num += spaceSize;
            }
            num += characterSpacing;
        }
    }

    private void DrawLetter(VertexHelper vh, int sprite, Vector2 pos)
    {
        pos -= inset;
        Sprite sprite2 = spriteSet[sprite];
        Vector2 vector = new Vector2(sprite2.rect.size.x, 0f - sprite2.rect.size.y);
        Vector2 vector2 = pos;
        Vector2 vector3 = vector + pos;
        vh.AddVert(new Vector2(vector2.x, vector2.y), color, new Vector2(sprite2.uv[0].x, sprite2.uv[0].y));
        vh.AddVert(new Vector2(vector2.x, vector3.y), color, new Vector2(sprite2.uv[0].x, sprite2.uv[3].y));
        vh.AddVert(new Vector2(vector3.x, vector3.y), color, new Vector2(sprite2.uv[3].x, sprite2.uv[3].y));
        vh.AddVert(new Vector2(vector3.x, vector2.y), color, new Vector2(sprite2.uv[3].x, sprite2.uv[0].y));
        int currentVertCount = vh.currentVertCount;
        vh.AddTriangle(currentVertCount - 1, currentVertCount - 2, currentVertCount - 3);
        vh.AddTriangle(currentVertCount - 3, currentVertCount - 4, currentVertCount - 1);
    }
}
