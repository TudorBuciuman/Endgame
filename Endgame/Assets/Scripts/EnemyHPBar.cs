using UnityEngine;
using UnityEngine.UI;

public class EnemyHPBar : MonoBehaviour
{
    private bool started;

    private RectTransform fgTransform;

    private int frames;

    private Vector2 oldSize;

    private Vector2 newSize;

    private float offset;

    private bool hitOnce;

    private int partyMembersAttacked;

    private void Update()
    {
        if (started)
        {
            frames++;
            fgTransform.sizeDelta = Vector2.Lerp(oldSize, newSize, (float)frames / 23f);
        }
    }

    protected virtual void LateUpdate()
    {
        if (started && frames == 43)
        {
            Object.Destroy(base.gameObject);
        }
    }

    public void StartHP(int dmg, int oldHp, int maxHp, int partyMember, bool mercy, bool emptyHPBarWhenZero = true)
    {
        frames = 0;
        int num = oldHp - dmg;
        if (num - oldHp != 0)
        {
            hitOnce = true;
        }
        if (mercy)
        {
            num = oldHp + dmg;
            if (num > 100)
            {
                num = 100;
            }
            if (oldHp > 100)
            {
                oldHp = 100;
            }
            //base.transform.Find("fg").GetComponent<Image>().color = PartyPanels.defaultColors[2];
        }
        base.transform.Find("fg").GetComponent<Image>().enabled = hitOnce;
        base.transform.Find("bg").GetComponent<Image>().enabled = hitOnce;
        base.transform.Find("bbg").GetComponent<Image>().enabled = hitOnce;
        if (num > maxHp)
        {
            num = maxHp;
        }
        fgTransform = base.transform.Find("fg").GetComponent<RectTransform>();
        if (!started)
        {
            oldSize = new Vector2((float)oldHp / (float)maxHp * base.transform.Find("bg").GetComponent<RectTransform>().sizeDelta[0], 13f);
        }
        else
        {
            oldSize = new Vector2(base.transform.Find("fg").GetComponent<RectTransform>().sizeDelta[0], 13f);
        }
        newSize = new Vector2((float)num / (float)maxHp * base.transform.Find("bg").GetComponent<RectTransform>().sizeDelta[0], 13f);
        fgTransform.sizeDelta = oldSize;
        if (!mercy)
        {
            if ((oldHp > 0 || !emptyHPBarWhenZero) && oldSize.x < 1f)
            {
                oldSize.x = 1f;
            }
            if ((num > 0 || !emptyHPBarWhenZero) && newSize.x < 1f)
            {
                newSize.x = 1f;
            }
        }
        started = true;
            EnemyHPText component = Object.Instantiate(Resources.Load<GameObject>("battle/enemies/EnemyHPText"), base.transform).GetComponent<EnemyHPText>();
            component.transform.localPosition = Vector3.zero;
            component.transform.localScale = new Vector3(1f, 1f, 1f);
            component.StartHP(dmg, oldHp, maxHp, partyMember, mercy, this);
        partyMembersAttacked++;
    }

    public void StartHP(int oldHp, int newHp, int maxHp, int partyMember, int width, bool mercy, bool emptyHPBarWhenZero = true)
    {
        base.transform.Find("bg").GetComponent<RectTransform>().sizeDelta = new Vector2(width, 13f);
        base.transform.Find("bbg").GetComponent<RectTransform>().sizeDelta = new Vector2(width + 2, 15f);
        base.transform.Find("fg").GetComponent<RectTransform>().localPosition = base.transform.Find("bg").GetComponent<RectTransform>().localPosition - new Vector3((float)width / 2f, 0f);
        StartHP(oldHp, newHp, maxHp, partyMember, mercy, emptyHPBarWhenZero);
        GameManager gameManager = Object.FindObjectOfType<GameManager>();
        if ((object)gameManager != null && gameManager.GetFlagInt(94) == 1 && hitOnce)
        {
            Image[] componentsInChildren = base.transform.Find("corners").GetComponentsInChildren<Image>();
            foreach (Image image in componentsInChildren)
            {
                image.enabled = true;
                int num = ((!(image.transform.localPosition.x < 0f)) ? 1 : (-1));
                image.transform.localPosition = new Vector2(width * num / 2, image.transform.localPosition.y);
            }
        }
    }

    public int GetNumPartyMembersEffecting()
    {
        return partyMembersAttacked;
    }
}
