using UnityEngine;

public class DashEffect : MonoBehaviour
{
    private float a = 0.75f;

    private void Update()
    {
        a -= 0.1f;
        Color color = GetComponent<SpriteRenderer>().color;
        color.a = a;
        GetComponent<SpriteRenderer>().color = color;
        if (a <= 0f)
        {
            Object.Destroy(base.gameObject);
        }
    }

    public void SetAttributes(Color color, SpriteRenderer sr)
    {
        if (!sr.enabled)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            a = 0f;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, a);
            GetComponent<SpriteRenderer>().sortingOrder = sr.sortingOrder - 1;
            GetComponent<SpriteRenderer>().flipY = sr.flipY;
        }
    }
}
