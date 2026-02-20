using UnityEngine;

public class BladeKnightBlasters : AttackBase
{
    private int length = 30;

    private int frequency;

    private bool[] isBlue;

    private int numEyeFlashes;

    private int numBlasters;

    private int blastStart = 65;

    private GameObject blastPrefab;

    private bool fadeIn;

    private int fadeFrames;

    protected override void Awake()
    {
        base.Awake();
        maxFrames = 1000;
        bbSize = new Vector2(165f, 140f);
        float num = (float)Object.FindObjectOfType<BladeKnight>().GetHP() / (float)Object.FindObjectOfType<BladeKnight>().GetMaxHP();
        if (num > 0.75f)
        {
            frequency = 2;
        }
        else if (num > 0.6f)
        {
            frequency = 3;
        }
        else if (num > 0.4f)
        {
            frequency = 4;
            length += 6;
            blastStart = 70;
        }
        else if (num > 0.2f)
        {
            frequency = 5;
            length += 10;
            blastStart = 75;
        }
        else
        {
            frequency = 6;
            length += 12;
            blastStart = 75;
        }
        isBlue = new bool[frequency];
        for (int i = 0; i < frequency; i++)
        {
            isBlue[i] = Random.Range(0, 2) == 1;
        }
    }

    protected override void Update()
    {
        if (!isStarted)
        {
            return;
        }
        frames++;
        if (frames >= 20 && numEyeFlashes < frequency && (frames - 20) % (length / frequency) == 0)
        {
            string path = ((numEyeFlashes == frequency - 1) ? "battle/attacks/bullets/lostcore/EyeFlash" : "battle/attacks/bullets/lostcore/EyeFlashSlight");
            Object.Instantiate(position: new Vector3((numEyeFlashes % 2 == 0) ? (-0.388f) : 0.56f, 3.42f), original: Resources.Load<GameObject>(path), rotation: Quaternion.identity).GetComponent<SpriteRenderer>().color = (isBlue[numEyeFlashes] ? new Color32(0, 162, 232, byte.MaxValue) : new Color32(252, 166, 0, byte.MaxValue));
            numEyeFlashes++;
        }
        if (frames >= blastStart && numBlasters < frequency && (frames - blastStart) % (length / frequency * 2) == 0)
        {
            float num = ((numBlasters % 2 == 0) ? 1 : (-1));
            //GasterBlaster component = Object.Instantiate(blastPrefab, new Vector3(-12f * num, 0f), Quaternion.Euler(0f, 0f, -90f * num)).GetComponent<GasterBlaster>();
            //component.SetBaseDamage(6);
            //component.ChangeType(isBlue[numBlasters] ? 1 : 2);
            //component.Activate(5, 5, 90f * num, new Vector2(-4.48f * num, -1.68f), length / frequency * 2 - (9 - frequency));
            numBlasters++;
        }
        if (!fadeIn)
        {
            return;
        }
        fadeFrames++;
        SpriteRenderer[] componentsInChildren = Object.FindObjectOfType<BladeKnight>().GetEnemyObject().transform.Find("parts").GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer spriteRenderer in componentsInChildren)
        {
            if (spriteRenderer.gameObject.name != "projectile" && spriteRenderer.gameObject.name != "swing")
            {
                spriteRenderer.color = new Color(1f, 1f, 1f, (float)fadeFrames / 15f);
            }
        }
        if (fadeFrames == 15)
        {
            Object.Destroy(base.gameObject);
        }
    }

    public void FadeIn()
    {
        fadeIn = true;
        Object.FindObjectOfType<BladeKnight>().transform.position = new Vector3(0f, Object.FindObjectOfType<BladeKnight>().transform.position.y);
        SpriteRenderer[] componentsInChildren = Object.FindObjectOfType<BladeKnight>().GetEnemyObject().transform.Find("parts").GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < componentsInChildren.Length; i++)
        {
            componentsInChildren[i].color = new Color(1f, 1f, 1f, 0f);
        }
    }

    public override void StartAttack()
    {
        base.StartAttack();
        blastPrefab = Resources.Load<GameObject>("battle/attacks/bullets/GasterBlaster");
        Object.FindObjectOfType<BladeKnight>().transform.position = new Vector3(100f, Object.FindObjectOfType<BladeKnight>().transform.position.y);
        Object.Instantiate(Resources.Load<GameObject>("battle/attacks/bullets/lostcore/BladeFigureBullet"));
    }
}
