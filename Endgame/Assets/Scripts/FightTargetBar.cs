using UnityEngine;

public class FightTargetBar : MonoBehaviour
{
    protected float multiplier = 1.25f;

    protected int partyMember;

    protected bool activated;

    protected bool ending;

    protected bool disabled = true;

    protected bool miss;

    protected int frames;

    protected int endFrames;

    protected Color initialColor = Color.white;

    protected Color endColor = new Color(1f, 1f, 1f, 0f);

    protected bool enemyAlive = true;

    protected EnemyBase enemy;

    protected float scoreMultiplier = 1f;

    protected virtual void Update()
    {
        if (activated)
        {
            frames++;
            if ((float)frames <= 40f * multiplier)
            {
                UpdatePosition();
            }
            else
            {
                base.transform.localPosition += new Vector3(-0.247f / multiplier, 0f);
            }
            if ((double)base.transform.localPosition.x < -5.833)
            {
                GetComponent<SpriteRenderer>().enabled = false;
                activated = false;
                ending = true;
                if ((bool)enemy)
                {
                    enemy.Hit(partyMember, 0f, playSound: false);
                    miss = true;
                }
            }
        }
        if (!ending || endFrames >= 14)
        {
            return;
        }
        endFrames++;
        Color color = Color.Lerp(initialColor, endColor, (float)endFrames / 6f);
        color.a = 1f - (float)endFrames / 12f;
        GetComponent<SpriteRenderer>().color = color;
        base.transform.localScale *= 1.1f;
        if (scoreMultiplier == 0f &&(bool)enemy && endFrames == 1)
        {
            enemy.Hit(partyMember, 0f, playSound: false);
            miss = true;
        }
        else if (endFrames >8 && !miss && !disabled)
        {
            enemy.Hit(0, GetSuccessRate(), true);
            disabled = true;
        }
        FindFirstObjectByType<FightTarget>().EndedFaster();

    }

    protected virtual void UpdatePosition()
    {
        base.transform.localPosition = new Vector3(Mathf.Lerp(8.303f, -4.05f, (float)(frames + 10) / (40f * multiplier + 10f)), base.transform.localPosition.y);
    }

    public virtual void Activate()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        disabled = false;
        activated = true;
        frames = Random.Range(0, 4) * 4;
    }

    public virtual void Activate(int forceFrames)
    {
        Activate();
        frames = forceFrames;
    }

    public void SetMultiplier(float multiplier)
    {
        this.multiplier = multiplier;
    }

    public virtual bool PushZ(bool enemyAlive)
    {
        this.enemyAlive = enemyAlive;
        activated = false;
        ending = true;
        if (enemyAlive)
        {
            enemy.SetPredictedDamage(partyMember, GetSuccessRate());
        }
        initialColor = GetComponent<SpriteRenderer>().color;
        endColor = GetComponent<SpriteRenderer>().color;
        endColor.a = 0f;
        if (GetSuccessRate() == 20f)
        {
            GetComponent<AudioSource>().Play();
            endColor = new Color(1f, 0.6f, 0f, 0f);
        }
        return true;
    }

    public EnemyBase GetEnemy()
    {
        return enemy;
    }

    public virtual void AssignValues(EnemyBase enemy, int partyMember)
    {
        this.enemy = enemy;
        this.partyMember = partyMember;
    }

    public virtual int GetCurFrames()
    {
        return Mathf.RoundToInt((float)frames / multiplier);
    }

    public virtual int GetLastFrames()
    {
        return frames;
    }

    public virtual bool CanPushZ()
    {
        if (frames > 16)
        {
            return activated;
        }
        return false;
    }

    public bool IsCompleted()
    {
        if (!ending)
        {
            return disabled;
        }
        return true;
    }

    public virtual bool IsMiss()
    {
        return miss;
    }
    public bool IsDisabled()
    {
        return disabled;
    }

    public void SetScoreMultiplier(float scoreMultiplier)
    {
        this.scoreMultiplier = scoreMultiplier;
    }

    public virtual float GetSuccessRate()
    {
        if (miss)
        {
            return 0f;
        }
        if ((float)frames == 40f * multiplier)
        {
            return 20f * scoreMultiplier;
        }
        if ((float)frames < 40f * multiplier)
        {
            return ((float)frames / multiplier / 2f - 5f) * scoreMultiplier;
        }
        if ((float)frames > 40f * multiplier)
        {
            float num = Mathf.Abs(base.transform.localPosition.x) - 4.05f;
            return (15f - num * 5f) * scoreMultiplier;
        }
        return 0f;
    }
}
