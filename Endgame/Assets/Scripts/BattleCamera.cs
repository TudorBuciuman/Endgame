using System;
using UnityEngine;

public class BattleCamera : MonoBehaviour
{
    private int frames;

    private int doShake;

    private void Start()
    {
        frames = 0;
        doShake = 0;
    }

    private void Update()
    {
        if (doShake == 1)
        {
            frames++;
            float num = 0f;
            if (frames == 1 || frames == 2)
            {
                num = 2f;
            }
            else if (frames == 5 || frames == 6)
            {
                num = 1f;
            }
            else if (frames >= 7)
            {
                doShake = 0;
                frames = 0;
            }
            base.transform.position = new Vector3(1f / 48f * num, 1f / 48f * (0f - num), -10f);
        }
        else if (doShake == 2)
        {
            frames++;
            float f = UnityEngine.Random.Range(0f, (float)Math.PI * 2f);
            if (frames == 1)
            {
                base.transform.position = new Vector3(Mathf.Cos(f) * -0.12f, Mathf.Sin(f) * -0.12f, -10f);
                return;
            }
            if (frames == 2)
            {
                base.transform.position = new Vector3(Mathf.Cos(f) * -0.06f, Mathf.Sin(f) * -0.06f, -10f);
                return;
            }
            base.transform.position = new Vector3(0f, 0f, -10f);
            doShake = 0;
            frames = 0;
        }
        else if (doShake == 3)
        {
            frames++;
            float f2 = UnityEngine.Random.Range(0f, (float)Math.PI * 2f);
            if (frames == 1)
            {
                base.transform.position = new Vector3(Mathf.Cos(f2) * -0.32f, Mathf.Sin(f2) * -0.32f, -10f);
                return;
            }
            if (frames == 2)
            {
                base.transform.position = new Vector3(Mathf.Cos(f2) * -0.18f, Mathf.Sin(f2) * -0.18f, -10f);
                return;
            }
            if (frames == 3)
            {
                base.transform.position = new Vector3(Mathf.Cos(f2) * -0.06f, Mathf.Sin(f2) * -0.06f, -10f);
                return;
            }
            base.transform.position = new Vector3(0f, 0f, -10f);
            doShake = 0;
            frames = 0;
        }
    }

    public void HurtShake()
    {
        if (doShake < 2)
        {
            doShake = 1;
            frames = 0;
            base.transform.position = new Vector3(0f, 0f, -10f);
        }
    }

    public void BlastShake()
    {
        doShake = 2;
        frames = 0;
    }

    public void GiantBlastShake()
    {
        doShake = 3;
        frames = 0;
    }
}
