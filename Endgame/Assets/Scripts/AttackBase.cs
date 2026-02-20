using UnityEngine;

public class AttackBase : TranslatableBehaviour
{
    protected int frames;

    protected int maxFrames;

    protected int state;

    protected bool isStarted;

    protected bool attackAllTargets = true;

    protected Vector2 bbSize;

    protected Vector2 bbPos;

    protected Vector3 soulPos;

    protected BulletBoard bb;

    protected virtual void Awake()
    {
        stringSubFolder = "attacks";
        frames = 0;
        maxFrames = 0;
        state = 0;
        isStarted = false;
        bbSize = new Vector2(300f, 140f);
        bbPos = new Vector2(0f, -1.66f);
        soulPos = new Vector2(-0.055f, -1.63f);
        bb = FindFirstObjectByType<BulletBoard>();
    }

    protected virtual void Update()
    {
        if (isStarted)
        {
            frames++;
            if (frames >= maxFrames)
            {
                Object.Destroy(base.gameObject);
            }
        }
    }

    public virtual void StartAttack()
    {
        isStarted = true;
    }

    public void StartAttack(int newMaxFrames)
    {
        maxFrames = newMaxFrames;
        StartAttack();
    }

    public bool HasStarted()
    {
        return isStarted;
    }

    public Vector2 GetBoardSize()
    {
        return bbSize;
    }

    public Vector2 GetBoardPos()
    {
        return bbPos;
    }

    public Vector3 GetSoulPos()
    {
        return soulPos;
    }

    public bool AttackingAllTargets()
    {
        return attackAllTargets;
    }
}
