using System.Collections.Generic;
using UnityEngine;

public class SOULGraze : MonoBehaviour
{
    private SOUL soul;

    private List<GameObject> bullets = new List<GameObject>();

    private Color baseColor = Color.white;

    private int whiteDecayFrames;

    private int grazeFrames;

    private int showFrames;

    private float tpBuildup;

    private float sizeMultiplier = 1f;

    private SpriteRenderer big;

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("battle/spr_soul_graze");
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
        GetComponent<CircleCollider2D>().isTrigger = true;
        big = new GameObject("BigGrazeGraphic").AddComponent<SpriteRenderer>();
        big.transform.parent = base.transform;
        big.transform.localPosition = Vector3.zero;
        big.sprite = Resources.Load<Sprite>("battle/spr_soul_graze");
        big.color = new Color(1f, 1f, 1f, 0f);
        UpdateGrazeSize();
        GetComponent<AudioSource>().playOnAwake = false;
        GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("sounds/snd_graze");
        base.gameObject.layer = 2;
    }

    private void Update()
    {
        if (soul.GetInvFrames() == 0)
        {
            for (int i = 0; i < bullets.Count; i++)
            {
                if (bullets[i] != null && (bool)bullets[i].GetComponent<BulletBase>())
                {
                    tpBuildup += bullets[i].GetComponent<BulletBase>().GetTPBuildRate();
                }
            }
        }
        AddExcessTP();
        List<GameObject> list = new List<GameObject>();
        foreach (GameObject bullet in bullets)
        {
            if (bullet == null)
            {
                list.Add(bullet);
            }
        }
        foreach (GameObject item in list)
        {
            bullets.Remove(item);
        }
        if (whiteDecayFrames < 4)
        {
            whiteDecayFrames++;
        }
    }

    private void LateUpdate()
    {
        if (bullets.Count > 0)
        {
            showFrames = 6;
        }
        else if (showFrames != 0)
        {
            showFrames--;
        }
        baseColor = soul.GetSOULColor() + Color.white / 2f;
        baseColor.a = 0.8f;
        baseColor = Color.Lerp(Color.white, baseColor, (float)whiteDecayFrames / 4f);
        Color a = baseColor;
        a.a = 0f;
        GetComponent<SpriteRenderer>().color = Color.Lerp(a, baseColor, (float)showFrames / 6f);
        GetComponent<SpriteRenderer>().sortingOrder = soul.GetComponent<SpriteRenderer>().sortingOrder;
        if (sizeMultiplier > 1f)
        {
            big.color = GetComponent<SpriteRenderer>().color;
            big.sortingOrder = soul.GetComponent<SpriteRenderer>().sortingOrder;
        }
        GetComponent<SpriteRenderer>().enabled = soul.GetComponent<SpriteRenderer>().enabled;
        big.enabled = GetComponent<SpriteRenderer>().enabled && sizeMultiplier > 1f;
    }

    public void AddTPBuildup(float tp)
    {
        tpBuildup += tp;
        AddExcessTP();
    }

    private void AddExcessTP()
    {
        while (tpBuildup >= 1f)
        {
            tpBuildup -= 1f;
            //Object.FindObjectOfType<TPBar>().AddTP(1);
        }
    }

    public void SetParentSOUL(SOUL soul)
    {
        this.soul = soul;
    }

    public void UpdateGrazeSize()
    {
        GetComponent<CircleCollider2D>().radius = 0.45f;
        big.transform.localScale = new Vector3(sizeMultiplier, sizeMultiplier, 1f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Contains("Bullet") && collision.gameObject.tag != "GreenBullet" && soul.GetInvFrames() == 0)
        {
            bullets.Add(collision.gameObject);
            if ((bool)collision.GetComponent<BulletBase>())
            {
                tpBuildup += collision.GetComponent<BulletBase>().GetTPGrazeValue();
            }
            else
            {
                tpBuildup += 1;
            }
            AddExcessTP();
            GetComponent<SpriteRenderer>().color = Color.white;
            GetComponent<AudioSource>().Play();
            showFrames = 6;
            whiteDecayFrames = -2;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (bullets.Contains(collision.gameObject))
        {
            bullets.Remove(collision.gameObject);
        }
    }
}
