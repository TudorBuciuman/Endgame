using UnityEngine;

public class ActionSOUL : MonoBehaviour
{
    private PlayerController player;

    private int fadeFrames;

    private bool activated;

    private bool hurt;

    private int hurtFrames;

    private bool restoreMovement;

    [SerializeField]
    private int inv = 15;

    private void Start()
    {
        player = Object.FindObjectOfType<PlayerController>();
    }

    protected virtual void Update()
    {
        if (activated && fadeFrames < 12)
        {
            fadeFrames++;
        }
        else if (!activated && fadeFrames > 0)
        {
            fadeFrames--;
        }
        int flagInt = Util.GameManager().GetFlagInt(312);
        Color sOULColorByID = SOUL.GetSOULColorByID(flagInt);
        if (!GetComponent<SpriteRenderer>().material.name.EndsWith(flagInt.ToString()))
        {
            GetComponent<SpriteRenderer>().material = Resources.Load<Material>("overworld/actionsoulpalettes/mat_actionsoul_" + flagInt);
        }
        GetComponent<SpriteRenderer>().color = Color.Lerp(new Color(1f, 1f, 1f, 0f), new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 180), (float)fadeFrames / 12f);
        base.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.Lerp(new Color(sOULColorByID.r, sOULColorByID.g, sOULColorByID.b, 0f), sOULColorByID, (float)fadeFrames / 12f);
        if (hurt && hurtFrames < inv)
        {
            hurtFrames++;
            if (((hurtFrames == 3 && inv >= 3) || (hurtFrames == inv && inv < 3)) && !Object.FindObjectOfType<TextBox>() && restoreMovement)
            {
                restoreMovement = false;
                Object.FindObjectOfType<PlayerController>().SetMovement(newMove: true);
            }
            if (hurtFrames == inv)
            {
                hurt = false;
                hurtFrames = 0;
            }
        }
    }

    protected virtual void LateUpdate()
    {
        base.transform.position = player.transform.position;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Contains("Bullet") && collision.gameObject.layer != 2 && !hurt && collision.gameObject.tag == "Bullet")
        {
            Damage(collision.gameObject.GetComponentInParent<BulletBase>().GetBaseDamage());
            collision.gameObject.GetComponentInParent<BulletBase>().SOULHit();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        OnTriggerEnter2D(collision);
    }

    public void Damage(int hp)
    {
        if (hurt)
        {
            return;
        }
        hurt = true;
        hurtFrames = 0;
        GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("sounds/snd_hurt");
        GetComponent<AudioSource>().Play();
        bool[] array = new bool[1]
        {
            true
        };
        Transform array2 = player.transform;
        int array3 = Util.GameManager().HandleDamageCalculations(hp, 1f, applyDamageImmediately: false);
        bool flag = false;
        for (int i = 0; i < 1; i++)
        {
            if (array3 > 0 && array[i])
            {
                flag = true;
            }
        }
        for (int j = 0; j < 1; j++)
        {
            if (array[j])
            {
                int num = ((array3 <= 0 && flag) ? 1 : array3);
                Util.GameManager().SetHP(num);
            }
        }
        if (Object.FindObjectOfType<PlayerController>().CanMove())
        {
            restoreMovement = true;
            Object.FindObjectOfType<PlayerController>().SetMovement(newMove: false);
        }
        if ((bool)Object.FindObjectOfType<ActionPartyPanels>())
        {
            Object.FindObjectOfType<ActionPartyPanels>().Raise();
            Object.FindObjectOfType<ActionPartyPanels>().UpdateHP(Object.FindObjectOfType<GameManager>().GetHP());
        }
        Object.FindObjectOfType<CameraController>().StartHitShake();
    }

    public void SetActivated(bool activated)
    {
        this.activated = activated;
    }
}
