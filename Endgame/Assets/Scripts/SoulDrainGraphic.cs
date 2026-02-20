using UnityEngine;

public class SOULDrainGraphic : MonoBehaviour
{
    [SerializeField]
    private Sprite[] sprites;

    private void LateUpdate()
    {
        if ((bool)base.transform.parent)
        {
            GetComponent<SpriteRenderer>().enabled = base.transform.parent.GetComponent<SpriteRenderer>().enabled;
            GetComponent<SpriteRenderer>().sortingOrder = base.transform.parent.GetComponent<SpriteRenderer>().sortingOrder + 1;
        }
    }

    public void SetGraphicType(int id, Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
        if (id < 0)
        {
            id = 0;
        }
        else if (id > 16)
        {
            id = 16;
        }
        GetComponent<SpriteRenderer>().sprite = sprites[id];
    }
}
