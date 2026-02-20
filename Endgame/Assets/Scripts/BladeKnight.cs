using UnityEngine;

public class BladeKnight : EnemyBase
{
    private Sprite[] hairSprites;

    private Sprite[] capeSprites;

    private int lastAct;

    private int talk = -1;

    private int bodyFrames;

    private bool hasPrayed;

    protected override void Awake()
    {
        base.Awake();
        enemyName = "BLADEKNIGHT";
        fileName = "bladeknight";
        maxHp = 200;
        hp = maxHp;
        hpPos = new Vector2(2f, 50f);
        hpWidth = 200;
        atk = 6;
        def = -10;
        flavorTxt = new string[5] { "* BLADEKNIGHT shines its pure red\n  sword.", "* BLADEKNIGHT stands motionless.", "* Its lack of facial features\n  disturbs you.", "* Smells like metal and rust.", "* It stands tall above you." };
        dyingTxt = new string[1] { "* It appears to be fading away." };
        satisfyTxt = new string[1] { "* BLADEKNIGHT doesn't want to\n  fight anymore." };
        chatter = new string[3] { "SURRENDER YOUR \nSOUL", "I'VE FELT THIS \nBEFORE", "GO FORWARD \nBRINGER OF HOPE" };
        actNames = new string[3] { "Talk", "Pray", "Scream" };
        canSpareViaFight = false;
        exp = 10;
        gold = 10;
        defaultChatPos = new Vector2(184f, 137f);
        attacks = new int[2] { 0, 1 };
        hairSprites = new Sprite[3];
        capeSprites = new Sprite[2];
        for (int i = 0; i < 3; i++)
        {
            hairSprites[i] = Resources.Load<Sprite>("battle/enemies/BLADEKNIGHT/spr_b_bladeknight_hair_2_" + i);
            if (i < 2)
            {
                capeSprites[i] = Resources.Load<Sprite>("battle/enemies/BLADEKNIGHT/spr_b_bladeknight_cape_" + i);
            }
        }
        hurtSpriteName = "_dmg_3";
    }

    protected void FixedUpdate()
    {
        base.Update();
        if (!gotHit)
        {
            bodyFrames++;
            GetPart("hair").GetComponent<SpriteRenderer>().sprite = hairSprites[bodyFrames / 4 % 3];
            GetPart("cape").GetComponent<SpriteRenderer>().sprite = capeSprites[bodyFrames / 6 % 2];
        }
    }

    public override void Chat(string[] text, string type, string sound, Vector2 pos, bool canSkip, int speed)
    {
        sound = "snd_txtwdc";
        if (talk > -1)
        {
            base.Chat(new string[1] { chatter[talk] }, type, sound, pos, canSkip, speed);
        }
        talk = -1;
    }

    public override void TurnToDust()
    {
        aud.clip = Resources.Load<AudioClip>("sounds/snd_deathnoise");
        aud.Play();
        CombineParts();
        obj.transform.Find("mainbody").GetComponent<ParticleDuplicator>().Activate(1);
    }

    public override string[] PerformAct(int i)
    {
        Debug.Log("acts");
        lastAct = i - 1;
        if (GetActNames()[i] == "Check")
        {
            return new string[1] { "* BLADEKNIGHT ATK 50 DEF 50\n* It has taken arms against\n  the light." };
        }
        if (GetActNames()[i] == "Pray")
        {
            def -= 3;
            if (satisfied < 100)
            {
                AddActPoints(50);
            }
            if (satisfied == 100)
            {
                talk = 1;
            }
            if (!hasPrayed)
            {
                hasPrayed = true;
                if ((int)Util.GameManager().GetFlag(13) < 3)
                {
                    return new string[1] { "* You prayed for safety.\n* Its DEFENSE decreased!" };
                }
                return new string[1] { "* You prayed for power.\n* Its DEFENSE decreased!" };
            }
            return new string[1] { "* You continue praying.\n* Its DEFENSE decreased!" };
        }
        if (GetActNames()[i] == "Talk")
        {
            if (satisfied == 100)
            {
                talk = 2;
            }
            else
            {
                talk = 0;
            }
            return new string[1] { "* You tried to talk to\n  BLADEKNIGHT,^05 but it responded\n  in a strange language." };
        }
        if (GetActNames()[i] == "Scream")
        {
            return new string[1] { "* You called for help.^05\n* But nobody came." };
        }
        return base.PerformAct(i);
    }
}
