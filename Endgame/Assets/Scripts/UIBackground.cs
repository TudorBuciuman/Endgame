using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIBackground : MonoBehaviour
{
    private string objName;

    private Vector2 pos;

    private Vector2 size;

    private GameObject border;

    private GameObject box;

    private bool exists;

    private bool ts;

    private bool bnp;

    public static Color[] borderColors = new Color[12]
    {
        Color.white,
        new Color(0.5f, 1f, 1f),
        new Color(1f, 0.5f, 1f),
        new Color(1f, 1f, 0.5f),
        new Color(1f, 0.75f, 0.5f),
        new Color(0.5f, 0.5f, 1f),
        new Color(1f, 0.5f, 0.5f),
        new Color(0.5f, 1f, 0.5f),
        new Color32(71, 64, 111, byte.MaxValue),
        new Color(0.5f, 0.75f, 1f),
        new Color(0.75f, 0.5f, 1f),
        new Color(0.5f, 0.5f, 0.5f)
    };

    private void Awake()
    {
        exists = false;
        //ts = (int)Util.GameManager().GetFlag(94) == 1;
        bnp = SceneManager.GetActiveScene().buildIndex == 123;
    }

    public void setUpInfo(string defName, Vector2 defPos, Vector2 defSize)
    {
        objName = defName;
        pos = defPos;
        size = defSize;
    }

    public void CreateElement(string defName, Vector2 defPos, Vector2 defSize)
    {
        setUpInfo(defName, defPos, defSize);
        CreateElement();
    }

    public void CreateElement()
    {
        if (exists)
        {
            return;
        }
        if (ts || bnp)
        {
            int num = (bnp ? 12 : 16);
            if (bnp)
            {
                GameObject obj = new GameObject(objName + "BBorderHori", typeof(RectTransform), typeof(Image));
                obj.transform.SetParent(base.transform, worldPositionStays: false);
                obj.transform.localPosition = pos;
                obj.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(size.x + 4f, size.y - (float)num + 4f);
                obj.GetComponent<Image>().color = Color.white;
                GameObject obj2 = new GameObject(objName + "BBorderVert", typeof(RectTransform), typeof(Image));
                obj2.transform.SetParent(base.transform, worldPositionStays: false);
                obj2.transform.localPosition = pos;
                obj2.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(size.x - (float)num + 4f, size.y + 4f);
                obj2.GetComponent<Image>().color = Color.white;
            }
            GameObject obj3 = new GameObject(objName + "BorderHori", typeof(RectTransform), typeof(Image));
            obj3.transform.SetParent(base.transform, worldPositionStays: false);
            obj3.transform.localPosition = pos;
            obj3.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(size.x, size.y - (float)num);
            obj3.GetComponent<Image>().color = Color.white;
            GameObject obj4 = new GameObject(objName + "BorderVert", typeof(RectTransform), typeof(Image));
            obj4.transform.SetParent(base.transform, worldPositionStays: false);
            obj4.transform.localPosition = pos;
            obj4.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(size.x - (float)num, size.y);
            obj4.GetComponent<Image>().color = Color.white;
        }
        else
        {
            border = new GameObject(objName + "Border");
            border.AddComponent<RectTransform>();
            border.AddComponent<Image>().color = Color.white;
            border.transform.SetParent(base.transform, worldPositionStays: false);
            border.transform.localPosition = pos;
            border.transform.GetComponent<RectTransform>().sizeDelta = size;
        }
        Vector2 sizeDelta = new Vector2(size[0] - 12f, size[1] - 12f);
        box = new GameObject(objName + "Box");
        box.AddComponent<RectTransform>();
        box.AddComponent<Image>();
        box.transform.SetParent(base.transform, worldPositionStays: false);
        box.transform.localPosition = pos;
        box.transform.GetComponent<RectTransform>().sizeDelta = sizeDelta;
        box.transform.GetComponent<Image>().color = new Color(0f, 0f, 0f);
        if (bnp)
        {
            Vector2[] array = new Vector2[4]
            {
                new Vector2(0f - size.x + 18f, size.y - 18f) / 2f,
                new Vector2(0f - size.x + 18f, 0f - size.y + 18f) / 2f,
                new Vector2(size.x - 18f, 0f - size.y + 18f) / 2f,
                new Vector2(size.x - 18f, size.y - 18f) / 2f
            };
            for (int i = 0; i < 4; i++)
            {
                GameObject obj5 = new GameObject(objName + "BorderCorner" + i, typeof(RectTransform), typeof(Image));
                obj5.GetComponent<RectTransform>().sizeDelta = new Vector2(6f, 6f);
                obj5.transform.SetParent(base.transform, worldPositionStays: false);
                obj5.transform.localPosition = pos + array[i];
                obj5.GetComponent<Image>().color = Color.white;
            }
        }
        else if (ts)
        {
            Sprite sprite = Resources.Load<Sprite>("ui/spr_rounded_corner");
            Vector2[] array2 = new Vector2[4]
            {
                new Vector2(0f - size.x + 16f, size.y - 16f) / 2f,
                new Vector2(0f - size.x + 16f, 0f - size.y + 16f) / 2f,
                new Vector2(size.x - 16f, 0f - size.y + 16f) / 2f,
                new Vector2(size.x - 16f, size.y - 16f) / 2f
            };
            for (int j = 0; j < 4; j++)
            {
                float z = 90 * j;
                GameObject obj6 = new GameObject(objName + "BorderCorner" + j, typeof(RectTransform), typeof(Image));
                obj6.GetComponent<Image>().sprite = sprite;
                obj6.GetComponent<RectTransform>().sizeDelta = new Vector2(16f, 16f);
                obj6.transform.SetParent(base.transform, worldPositionStays: false);
                obj6.transform.localPosition = pos + array2[j];
                obj6.transform.eulerAngles = new Vector3(0f, 0f, z);
                obj6.GetComponent<Image>().color = Color.white;
            }
        }
        base.transform.localPosition = new Vector2(0f, 0f);
        base.transform.localScale = new Vector3(1f, 1f, 1f);
        exists = true;
    }
}
