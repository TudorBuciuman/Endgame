using System;
using UnityEngine;

public class SOULChargeEffect : MonoBehaviour
{
    private bool activated;

    private int frames;

    private SOUL parentSOUL;

    private bool doParryInv;

    private Color color;

    private void LateUpdate()
    {
        if (!activated)
        {
            return;
        }
        frames++;
        if (!doParryInv)
        {
            GetComponent<AudioSource>().volume = Mathf.Lerp(0f, 0.75f, (float)frames / 5f);
            GetComponent<AudioSource>().pitch = Mathf.Lerp(0.75f, 1f, (float)frames / 5f);
        }
        else
        {
            color.a = Mathf.Lerp(0f, 90f, (float)parentSOUL.GetInvFrames() / 20f) / 255f;
            GetComponent<AudioSource>().volume = Mathf.Lerp(0f, 0.75f, (float)parentSOUL.GetInvFrames() / 20f);
            GetComponent<AudioSource>().pitch = Mathf.Lerp(0.75f, 1f, (float)parentSOUL.GetInvFrames() / 20f);
        }
        for (int i = 0; i < 3; i++)
        {
            float num = Mathf.Abs(Mathf.Sin(180f / (float)(30 + 10 * i) * (float)frames * ((float)Math.PI / 180f)));
            base.transform.GetChild(i).localScale = new Vector3(1f + num, 1f + num, 1f);
            if ((bool)parentSOUL)
            {
                base.transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = parentSOUL.GetComponent<SpriteRenderer>().enabled;
            }
            if (doParryInv)
            {
                base.transform.GetChild(i).GetComponent<SpriteRenderer>().color = color;
            }
        }
    }

    public void Activate(SOUL baseSOUL)
    {
        parentSOUL = baseSOUL;
        if ((bool)parentSOUL && parentSOUL.GetSOULMode() == 4)
        {
            doParryInv = true;
        }
        base.transform.localPosition = Vector3.zero;
        color = parentSOUL.GetSOULColor();
        color.a = 0.3529412f;
        for (int i = 0; i < 3; i++)
        {
            base.transform.GetChild(i).GetComponent<SpriteRenderer>().color = color;
            base.transform.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder = baseSOUL.GetComponent<SpriteRenderer>().sortingOrder - 1;
            base.transform.GetChild(i).GetComponent<SpriteRenderer>().flipY = baseSOUL.GetComponent<SpriteRenderer>().flipY;
        }
        activated = true;
    }
}
