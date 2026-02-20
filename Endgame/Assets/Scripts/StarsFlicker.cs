using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class StarFlickerManager : MonoBehaviour
{
    private int flickerInterval = 76;

    private float minAlpha = 0.3f;

    private float maxAlpha = 0.6f;

    private List<SpriteRenderer> stars = new List<SpriteRenderer>();
    private int frameCounter;

    void Start()
    {
        GameObject[] starObjects = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++) {
            stars.Add(transform.GetChild(i).GetComponent<SpriteRenderer>());
        }
        FindFirstObjectByType<GameManager>().SetMenuDis();
    }

    void Update()
    {
        frameCounter++;
        if (frameCounter >= flickerInterval)
        {
            FlickerStars();
            frameCounter = 0;
        }
    }

    void FlickerStars()
    {
        foreach (SpriteRenderer sr in stars)
        {
            float alpha = Random.Range(minAlpha, maxAlpha);
            if (Mathf.Abs(sr.color.a - alpha) < 0.3f)
            {
                Color color = sr.color;
                color.a = alpha;
                sr.color = color;
            }
        }
    }
}
