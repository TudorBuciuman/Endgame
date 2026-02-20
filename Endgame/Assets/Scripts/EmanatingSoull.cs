using System;
using UnityEngine;

public class EmanatingSOUL : MonoBehaviour
{
    private bool started;

    private SpriteRenderer sr;

    private float scaleVal;

    private bool reverse;

    private int frames;

    private void Awake()
    {
        started = false;
    }

    private void Update()
    {
        if (!started)
        {
            return;
        }
        frames++;
        if (frames > 20)
        {
            UnityEngine.Object.Destroy(base.gameObject);
            return;
        }
        scaleVal -= 0.025f;
        if (reverse && frames <= 10)
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.05f * (float)frames);
        }
        else
        {
            float num = (float)frames / 20f;
            num = Mathf.Sin(num * (float)Math.PI * 0.5f);
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, Mathf.Lerp(1f, 0f, num));
        }
        if (reverse)
        {
            base.transform.localScale -= new Vector3(scaleVal, scaleVal);
        }
        else
        {
            base.transform.localScale += new Vector3(scaleVal, scaleVal);
        }
    }

    public void StartEffect(bool doReverse, Color color, bool isMonster)
    {
        if (!started)
        {
            sr = base.gameObject.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 100;
            reverse = doReverse;
            scaleVal = 0.5f;
            if (reverse)
            {
                base.transform.localScale = new Vector2(5.7f, 5.7f);
                color = new Color(color.r, color.b, color.g, 0f);
            }
            frames = 0;
            sr.sprite = Resources.Load<Sprite>("battle/spr_soul");
            sr.flipY = isMonster;
            sr.color = color;
            started = true;
        }
    }
}
