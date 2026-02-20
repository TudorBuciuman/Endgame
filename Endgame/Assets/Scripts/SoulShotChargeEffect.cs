using System;
using UnityEngine;

public class SOULShotChargeEffect : MonoBehaviour
{
    private bool activated;

    private int frames;

    private SOUL parentSOUL;

    private Color color;

    private Transform balls;

    private void Awake()
    {
        balls = base.transform.Find("Balls");
    }

    private void Update()
    {
        if (!activated)
        {
            return;
        }
        frames++;
        color.a = Mathf.Lerp(0f, 90f, (float)parentSOUL.GetChargeFrames() / 20f) / 255f;
        if (parentSOUL.GetChargeFrames() >= 5)
        {
            GetComponent<AudioSource>().volume = Mathf.Lerp(0f, 0.75f, (float)(parentSOUL.GetChargeFrames() - 10) / 10f);
            GetComponent<AudioSource>().pitch = Mathf.Lerp(0.1f, 1f, (float)(parentSOUL.GetChargeFrames() - 10) / 10f);
            for (int i = 0; i < balls.childCount; i++)
            {
                Transform child = balls.GetChild(i);
                float num = (float)(parentSOUL.GetChargeFrames() - 5) / 15f;
                float num2 = (float)i * 90f + num * 135f;
                float num3 = (1f - num) / 1.25f;
                child.transform.localPosition = new Vector3(Mathf.Cos(num2 * ((float)Math.PI / 180f)) * num3, Mathf.Sin(num2 * ((float)Math.PI / 180f)) * num3, 0f);
                child.transform.localScale = Vector3.Lerp(new Vector3(4f, 4f, 0f), new Vector3(2f, 2f, 0f), num);
                child.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 0f, (parentSOUL.GetChargeFrames() <= 6) ? 0.5f : 1f);
            }
        }
        if (parentSOUL.GetChargeFrames() != 20)
        {
            return;
        }
        for (int j = 0; j < 3; j++)
        {
            float num4 = Mathf.Abs(Mathf.Sin(180f / (float)(30 + 10 * j) * (float)(frames - 15) * ((float)Math.PI / 180f)));
            base.transform.GetChild(j).GetComponent<SpriteRenderer>().enabled = true;
            base.transform.GetChild(j).localScale = new Vector3(1f + num4, 1f + num4, 1f);
            if ((bool)parentSOUL)
            {
                base.transform.GetChild(j).GetComponent<SpriteRenderer>().enabled = parentSOUL.GetComponent<SpriteRenderer>().enabled;
            }
        }
    }

    public void Activate(SOUL baseSOUL)
    {
        parentSOUL = baseSOUL;
        base.transform.localPosition = Vector3.zero;
        color = parentSOUL.GetSOULColor();
        color.a = 0.3529412f;
        for (int i = 0; i < 3; i++)
        {
            base.transform.GetChild(i).GetComponent<SpriteRenderer>().color = color;
            base.transform.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder = baseSOUL.GetComponent<SpriteRenderer>().sortingOrder - 1;
            base.transform.GetChild(i).GetComponent<SpriteRenderer>().flipY = true;
        }
        activated = true;
    }
}
