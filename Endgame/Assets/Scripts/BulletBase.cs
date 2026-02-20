using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(AudioSource))]
public class BulletBase : MonoBehaviour
{
    protected Dictionary<string, Color> colorDict = new Dictionary<string, Color>
    {
        {
            "Bullet",
            Color.white
        },
        {
            "BlueBullet",
            new Color32(0, 162, 232, byte.MaxValue)
        },
        {
            "OrangeBullet",
            new Color32(252, 166, 0, byte.MaxValue)
        },
        {
            "GreenBullet",
            new Color32(64, byte.MaxValue, 64, byte.MaxValue)
        }
    };

    protected string[] tagArray = new string[4] { "Bullet", "BlueBullet", "OrangeBullet", "GreenBullet" };

    protected int state;

    protected int frames;

    protected bool destroyOnHit;

    protected int baseDmg;

    protected int karmaImpact;

    protected float tpGrazeValue = 1f;

    protected float tpGrazeValueReuse = 0.1f;

    protected float tpBuildRate = 0.075f;

    protected bool grazed;

    protected bool canGetShot;

    protected SpriteRenderer sr;

    protected AudioSource aud;

    protected virtual void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        aud = GetComponent<AudioSource>();
        if ((bool)GetComponent<Collider2D>())
        {
            GetComponent<Collider2D>().isTrigger = true;
        }
        if (!base.gameObject.tag.Contains("Bullet"))
        {
            base.gameObject.tag = "Bullet";
        }
        base.gameObject.layer = 9;
        ChangeType(base.gameObject.tag);
        frames = 0;
        state = 0;
        destroyOnHit = true;
        baseDmg = 5;
        karmaImpact = 0;
    }

    public virtual void ChangeType(string tag)
    {
        base.gameObject.tag = tag;
        sr.color = colorDict[tag];
    }

    public void ChangeType(int id)
    {
        ChangeType(tagArray[id]);
    }

    public void PlaySFX(string sfx, float pitch = 1f)
    {
        aud.clip = Resources.Load<AudioClip>(sfx);
        aud.pitch = pitch;
        aud.Play();
    }

    protected void PlaySFX(string sfx)
    {
        PlaySFX(sfx, 1f);
    }

    public bool DoesDestroy()
    {
        return destroyOnHit;
    }

    public void SetDestroyOnHit(bool destroyOnHit)
    {
        this.destroyOnHit = destroyOnHit;
    }

    public void SetBaseDamage(int baseDmg)
    {
        this.baseDmg = baseDmg;
    }

    public int GetBaseDamage()
    {
        return baseDmg;
    }

    public void SetKarmaImpact(int karmaImpact)
    {
        this.karmaImpact = karmaImpact;
    }

    public int GetKarmaImpact()
    {
        return karmaImpact;
    }

    public virtual void SOULHit()
    {
        if (destroyOnHit)
        {
            Object.Destroy(base.gameObject);
        }
    }

    public virtual void PreSOULHit()
    {
    }

    public virtual void Parry()
    {
    }

    public virtual void GetShot(bool bigshot)
    {
    }

    public bool CanGetShot()
    {
        return canGetShot;
    }

    public void SetTPBuildRate(float tpBuildRate)
    {
        this.tpBuildRate = tpBuildRate;
    }

    public float GetTPBuildRate()
    {
        return tpBuildRate;
    }

    public void SetTPGrazeValue(float tpGrazeValue, bool setReuse = false)
    {
        if (setReuse)
        {
            tpGrazeValueReuse = tpGrazeValue;
        }
        else
        {
            this.tpGrazeValue = tpGrazeValue;
        }
    }

    public void SetGrazed(bool grazed)
    {
        this.grazed = grazed;
    }

    public float GetTPGrazeValue()
    {
        if (!grazed)
        {
            grazed = true;
            return tpGrazeValue;
        }
        return tpGrazeValueReuse;
    }

    public int GetBulletType()
    {
        for (int i = 0; i < tagArray.Length; i++)
        {
            if (base.gameObject.tag == tagArray[i])
            {
                return i;
            }
        }
        return 0;
    }
}
