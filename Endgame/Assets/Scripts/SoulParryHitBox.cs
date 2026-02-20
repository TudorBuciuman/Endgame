using UnityEngine;

public class SOULParryHitBox : MonoBehaviour
{
    private SOUL soul;

    private void Start()
    {
        base.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("battle/spr_soul");
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().isTrigger = true;
        GetComponent<BoxCollider2D>().size = new Vector2(1f, 1f);
        base.gameObject.layer = 2;
    }

    public void SetParentSOUL(SOUL soul)
    {
        this.soul = soul;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Contains("Bullet") && collision.gameObject.layer != 2)
        {
            soul.HandleParry();
            soul.HandleParryCollision(collision.GetComponent<BulletBase>());
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Contains("Bullet") && collision.gameObject.layer != 2)
        {
            soul.HandleParry();
            soul.HandleParryCollision(collision.GetComponent<BulletBase>());
        }
    }
}
