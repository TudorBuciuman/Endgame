using UnityEngine;

public class BladeFigureBullet : BulletBase
{
    private SpriteRenderer body;

    private SpriteRenderer arm;

    private SpriteRenderer fade;

    private float bulletX = -0.16f;

    protected override void Awake()
    {
        base.Awake();
        baseDmg = 10;
        destroyOnHit = false;
        body = base.transform.parent.Find("Body").GetComponent<SpriteRenderer>();
        arm = base.transform.parent.Find("Arm").GetComponent<SpriteRenderer>();
        fade = base.transform.parent.Find("WhiteFade").GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        frames++;
        if (frames <= 10)
        {
            fade.color = new Color(1f, 1f, 1f, 1f - (float)frames / 10f);
        }
        if (frames >= 85 && frames <= 95)
        {
            sr.color = new Color(1f, 1f, 1f, 1f - (float)(frames - 85) / 10f);
        }
        else if (frames >= 105 && frames <= 115)
        {
            if (frames == 105)
            {
                base.transform.localScale = new Vector3(1f, -1f);
                base.transform.position = new Vector3(-0.16f, 1.715f);
            }
            sr.color = new Color(1f, 1f, 1f, (float)(frames - 105) / 10f);
        }
        if (frames >= 125 && frames <= 185)
        {
            float num = (float)(frames - 125) / 60f;
            num = num * num * num * (num * (6f * num - 15f) + 10f);
            bulletX = Mathf.Lerp(base.transform.position.x, Object.FindObjectOfType<SOUL>().transform.position.x - 0.32f, num);
            base.transform.position = new Vector3(bulletX, Mathf.Lerp(1.715f, 2.47f, num));
            arm.transform.position = new Vector3(-0.16f, Mathf.Lerp(2.21f, 2.64f, num));
            if (frames == 185)
            {
                body.GetComponent<AudioSource>().Play();
                sr.sprite = Resources.Load<Sprite>("battle/attacks/bullets/lostcore/spr_blade_figure_bullet");
            }
        }
        if (frames >= 185 && frames < 200)
        {
            bulletX = Object.FindObjectOfType<SOUL>().transform.position.x - 0.32f;
            base.transform.position = new Vector3(bulletX, 2.47f);
        }
        if (frames >= 200)
        {
            if (frames == 200)
            {
                arm.GetComponent<AudioSource>().Play();
            }
            arm.transform.position = new Vector3(-0.16f, Mathf.Lerp(2.64f, 1.95f, (float)(frames - 200) / 5f));
            arm.color = new Color(1f, 1f, 1f, 1f - (float)(frames - 200) / 5f);
            body.color = new Color(1f, 1f, 1f, 1f - (float)(frames - 200) / 5f);
            base.transform.position = new Vector3(bulletX, Mathf.Lerp(2.47f, -1.78f, (float)(frames - 200) / 15f));
            if (base.transform.position.y <= -0.88f && sr.maskInteraction != SpriteMaskInteraction.VisibleInsideMask)
            {
                sr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                sr.sortingOrder = 197;
            }
            if (frames == 215)
            {
                GetComponent<AudioSource>().Play();
                Object.FindObjectOfType<BattleCamera>().GiantBlastShake();
            }
            if (frames == 235)
            {
                Object.FindObjectOfType<BladeKnightBlasters>().FadeIn();
            }
            sr.color = new Color(1f, 1f, 1f, 1f - (float)(frames - 235) / 15f);
            if (frames == 250)
            {
                Object.Destroy(base.transform.parent.gameObject);
            }
        }
    }
}
