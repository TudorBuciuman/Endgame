using UnityEngine;

public class SOULHitBox : MonoBehaviour
{
    private SOUL soul;

    private void Start()
    {
        base.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("battle/spr_soul");
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().isTrigger = true;
        GetComponent<BoxCollider2D>().size = new Vector2(1f / 3f, 1f / 3f);
        base.gameObject.layer = 2;
    }

    public void SetParentSOUL(SOUL soul)
    {
        this.soul = soul;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        soul.BulletTriggerEnter(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        soul.BulletTriggerStay(collision);
    }
}
