using System;
using UnityEngine;

public class SpareDust : MonoBehaviour
{
    private Vector3 startPos;

    private Vector3 finalPos;

    private int frames;

    private bool playing;

    private void Awake()
    {
        frames = 0;
        float num = UnityEngine.Random.Range(0.8f, 1f);
        base.transform.localScale = new Vector3(num, num, 1f);
    }

    private void Update()
    {
        if (playing)
        {
            frames++;
            if (frames <= 9)
            {
                float num = (float)frames / 9f;
                num = Mathf.Sin(num * (float)Math.PI * 0.5f);
                base.transform.position = Vector3.Lerp(startPos, finalPos, num);
            }
            if (frames == 12)
            {
                UnityEngine.Object.Destroy(base.gameObject);
            }
        }
    }

    public void StartMovement(Vector3 dir)
    {
        startPos = base.transform.position;
        finalPos = base.transform.position + dir * UnityEngine.Random.Range(0.6f, 1.5f);
        playing = true;
    }
}
