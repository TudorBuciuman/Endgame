using System;
using UnityEngine;

public class GasterBlaster : BulletBase
{
    [SerializeField]
    private int width = 1;

    [SerializeField]
    private int height = 2;

    [SerializeField]
    private float angle;

    [SerializeField]
    private Vector2 position = Vector2.zero;

    [SerializeField]
    private int blastFrames = 10;

    [SerializeField]
    private bool activated;

    private Vector3 origPosition;

    private float origAngle;

    private Transform blast;

    private bool isBlasting;

    private bool inSpearAttack;

    private int delayOffset;

    private Sprite[] sprites;

    protected override void Awake()
    {
        base.Awake();
        baseDmg = 1;
        karmaImpact = 10;
        origPosition = base.transform.position;
        origAngle = base.transform.rotation.eulerAngles.z;
        blast = base.transform.GetChild(0);
        destroyOnHit = false;
        tpGrazeValue = 2f;
        tpBuildRate = 0.2f;
        GetComponent<BoxCollider2D>().enabled = false;
        Resize(width, height);
        sr = GetComponent<SpriteRenderer>();
        sprites = new Sprite[6];
        for (int i = 0; i < 6; i++)
        {
            sprites[i] = Resources.Load<Sprite>("battle/attacks/bullets/spr_blaster_" + i);
        }
    }

    private void Update()
    {
        if (!activated)
        {
            return;
        }
        if (frames == 0)
        {
            if (GetBulletType() == 1)
            {
                GetComponent<AudioSource>().pitch -= 0.2f;
            }
            if (GetBulletType() == 2)
            {
                GetComponent<AudioSource>().pitch += 0.2f;
            }
            GetComponent<AudioSource>().Play();
        }
        frames++;
        if (frames <= 15)
        {
            float num = (float)frames / 15f;
            num -= 1f;
            num = 1f * (num * num * num * num * num + 1f) + 0f;
            base.transform.position = Vector3.Lerp(origPosition, position, num);
            base.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(origAngle, angle, num));
        }
        if (frames >= 28 + delayOffset && frames <= 30 + delayOffset)
        {
            sr.sprite = sprites[frames - (27 + delayOffset)];
        }
        if (frames <= 30 + delayOffset)
        {
            return;
        }
        float num2 = (float)(frames - (30 + delayOffset)) / 15f;
        num2 = 1f - Mathf.Cos(num2 * (float)Math.PI * 0.5f);
        if (frames >= 45 + delayOffset)
        {
            num2 = 1f;
        }
        Vector3 vector = new Vector3(0f - Mathf.Sin(angle * ((float)Math.PI / 180f)), Mathf.Cos(angle * ((float)Math.PI / 180f))) * 13f;
        base.transform.position = Vector3.Lerp(position, (Vector3)position + vector, num2);
        int num3 = Mathf.FloorToInt((float)(frames - 30 + delayOffset) / 6f) % 2 + 4;
        sr.sprite = sprites[num3];
        float num4 = (float)((frames - (32 + delayOffset)) % 8) / 4f;
        if (num4 >= 1f)
        {
            num4 = (float)(8 - (frames - (32 + delayOffset)) % 8) / 4f;
        }
        num4 = num4 * num4 * num4 * (num4 * (6f * num4 - 15f) + 10f);
        blast.localScale = Vector3.Lerp(new Vector3(1f, 1f, 1f), new Vector3(0.6f, 1f, 1f), num4);
        if (frames == 31 + delayOffset)
        {
            blast.GetComponent<AudioSource>().Play();
            GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("sounds/snd_blaster_blast");
            GetComponent<AudioSource>().volume = 0.65f * blast.GetComponent<AudioSource>().volume;
            GetComponent<AudioSource>().Play();
            SpriteRenderer[] componentsInChildren = blast.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer obj in componentsInChildren)
            {
                obj.enabled = true;
                obj.color = sr.color;
            }
            blast.localScale = new Vector3(0.2625f, 1f, 1f);
            GetComponent<BoxCollider2D>().enabled = true;
            if (width >= 3)
            {
                UnityEngine.Object.FindObjectOfType<BattleCamera>().GiantBlastShake();
            }
            else if (width > 1)
            {
                UnityEngine.Object.FindObjectOfType<BattleCamera>().BlastShake();
            }
        }
        else if (frames + 8 >= 30 + delayOffset + blastFrames)
        {
            num4 = (float)(frames + 8 - (30 + delayOffset + blastFrames)) / 8f;
            if (num4 >= 0.25f)
            {
                GetComponent<BoxCollider2D>().enabled = false;
                isBlasting = false;
            }
            num4 = Mathf.Sin(num4 * (float)Math.PI * 0.5f);
            if (frames >= 30 + delayOffset + blastFrames)
            {
                num4 = 1f;
            }
            blast.localScale = Vector3.Lerp(new Vector3(0.6f, 1f, 1f), new Vector3(0f, 1f, 1f), num4);
            Color color = Color.Lerp(sr.color, new Color(sr.color.r, sr.color.g, sr.color.b, 0f), num4);
            SpriteRenderer[] componentsInChildren = blast.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                componentsInChildren[i].color = color;
            }
            if (num4 == 1f && !GetComponent<AudioSource>().isPlaying && !blast.GetComponent<AudioSource>().isPlaying)
            {
                UnityEngine.Object.Destroy(base.gameObject);
            }
        }

            inSpearAttack = false;
            SpriteRenderer[] componentssInChildren = GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < componentssInChildren.Length; i++)
            {
                componentssInChildren[i].maskInteraction = SpriteMaskInteraction.None;
            }
    }

    public void Activate()
    {
        origPosition = base.transform.position;
        origAngle = base.transform.rotation.eulerAngles.z;
        activated = true;
        isBlasting = true;
    }

    public void Activate(int width, int height, float angle, Vector2 position, int blastFrames, bool inSpearAttack = false, int delayOffset = 0)
    {
        if (!activated)
        {
            Resize(width, height);
            this.angle = angle;
            this.position = position;
            this.blastFrames = blastFrames;
            this.inSpearAttack = inSpearAttack;
            this.delayOffset = delayOffset;
            Activate();
        }
    }

    private void Resize(int width, int height)
    {
        this.width = width;
        this.height = height;
        base.transform.localScale = new Vector3(width, height, 1f);
    }

    public void Mute()
    {
        blast.GetComponent<AudioSource>().volume = 0f;
        GetComponent<AudioSource>().volume = 0f;
    }

    public bool IsBlasting()
    {
        return isBlasting;
    }

    public Vector2 GetFaceDirection()
    {
        return new Vector2(Mathf.Sin((angle - 180f) * ((float)Math.PI / 180f)), Mathf.Cos(angle * ((float)Math.PI / 180f)));
    }
}
