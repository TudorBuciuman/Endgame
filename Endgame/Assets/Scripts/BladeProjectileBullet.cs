using UnityEngine;

public class BladeProjectileBullet : BulletBase
{
    private Vector3 dir;

    private float speed;

    [SerializeField]
    private Sprite[] sprites;

    protected override void Awake()
    {
        base.Awake();
        destroyOnHit = false;
        baseDmg = 7;
        base.transform.up = base.transform.position - Object.FindObjectOfType<SOUL>().transform.position;
        dir = Vector3.MoveTowards(base.transform.position, Object.FindObjectOfType<SOUL>().transform.position, 1f / 48f) - base.transform.position;
        speed = Mathf.Lerp(20f, 14f, (float)Object.FindObjectOfType<BladeKnight>().GetHP() / (float)Object.FindObjectOfType<BladeKnight>().GetMaxHP());
    }

    private void Update()
    {
        frames++;
        sr.sprite = sprites[frames / 3 % 3];
        base.transform.position += dir * speed;
        if (base.transform.position.y <= -5f || Mathf.Abs(base.transform.position.x) >= 6.66f)
        {
            SplitApart();
        }
    }

    private void SplitApart()
    {
        Object.FindObjectOfType<BattleCamera>().BlastShake();
        for (int i = 0; i < 3; i++)
        {
            float angle = base.transform.eulerAngles.z + 90f + 15f - (float)(15 * i);
            float num = -0.52f + 0.52f * (float)i;
            if (base.transform.position.x > 0f && base.transform.position.y > -5f)
            {
                num *= -1f;
            }
            Vector3 position = new Vector3(num + base.transform.position.x, -5f);
            bool flag = false;
            if (Mathf.Abs(base.transform.position.x) >= 6.66f)
            {
                flag = true;
                position = new Vector3(6.65f * Mathf.Sign(base.transform.position.x), num + base.transform.position.y);
            }
            BladeBounceBullet componentInChildren = Object.Instantiate(Resources.Load<GameObject>("battle/attacks/bullets/lostcore/BladeBounceBullet"), position, Quaternion.identity, base.transform.parent).GetComponentInChildren<BladeBounceBullet>();
            componentInChildren.SetValues(speed, angle);
            if (flag)
            {
                componentInChildren.transform.parent.localScale = new Vector3(-1f, 1f);
            }
            else
            {
                componentInChildren.transform.parent.localScale = new Vector3(1f, -1f);
            }
            if (i == 0)
            {
                componentInChildren.GetComponentInChildren<AudioSource>().Play();
            }
        }
        Object.Destroy(base.gameObject);
    }
}
