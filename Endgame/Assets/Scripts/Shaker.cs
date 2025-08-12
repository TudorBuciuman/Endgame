using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shaker : MonoBehaviour
{
    public float magnitude = 0.0025f;
    float X;
    float Y;
    void Awake()
    {
        X = transform.position.x;
        Y = transform.position.y;
    }
    public void Update()
    {
        float elapsed = 0.0f;

        while (elapsed < 600)
        {
            float x = X + Random.Range(-1f, 1f) * magnitude;
            float y = Y + Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
        }
    }
}
