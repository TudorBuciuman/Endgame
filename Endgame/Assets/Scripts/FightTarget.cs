using UnityEngine;

public class FightTarget : MonoBehaviour
{
    private EnemyBase[] enemies = new EnemyBase[3];

    private FightTargetBar bars;

    private bool attacking = false;

    private int startFrames;

    private int endFrames;

    private bool done;
    private bool ended = false;
    private void Awake()
    {
        if ((int)Object.FindObjectOfType<GameManager>().GetFlag(94) == 1)
        {
            base.transform.Find("Target").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("battle/spr_target_1_ts");
        }
            bars = Object.Instantiate(Resources.Load<GameObject>("battle/reticles/Standard"), base.transform).GetComponent<FightTargetBar>();
            SpriteRenderer[] componentsInChildren = bars.GetComponentsInChildren<SpriteRenderer>();
        ended = false;
        bars.Activate();
    }

    private void Update()
    {
        bool flag = false;
        if (!done && !flag && startFrames >= 5)
        {
            bool flag2 = false;
            FightTargetBar array = bars;
            if (!bars.IsCompleted())
            {
                flag2 = true;
            }
            if (!flag2)
            {
                done = true;
            }
        }
        if (startFrames < 5)
        {
            startFrames++;
            if (startFrames != 5)
            {
                return;
            }
            int num = 50;
            if (attacking)
            {        
                bars.Activate();
                if (bars.GetLastFrames() < num)
                {
                    num = bars.GetLastFrames();
                }
            }
            return;
        }
        bool flag3 = false;
        if (UTInput.GetButtonDown("Z") || flag3)
        {
            int num3 = 0;
            if (bars.GetCurFrames() > num3 && bars.CanPushZ())
            {
               num3 = bars.GetCurFrames();
            }
            for (int m = 0; m < 1; m++)
            {
                int num4 = m;
                int num5 = m;
                bool flag4 = true;
                if (enemies[num4].GetPredictedHP() <= 0)
                {
                    flag4 = false;
                    EnemyBase[] array2 = Object.FindObjectOfType<BattleManager>().GetEnemies();
                    foreach (EnemyBase enemyBase in array2)
                    {
                        if (enemyBase.GetPredictedHP() > 0)
                        {
                            bars.AssignValues(enemyBase, num5);
                            enemies[num4] = enemyBase;
                            flag4 = true;
                            break;
                        }
                    }
                }
                bool flag5 = bars.PushZ(flag4);
                if(flag5)
                PlayHitAnimation(enemies[num4], num5, bars.GetSuccessRate(), m);
                
            }
        }
        if (done)
        {
            endFrames++;
            base.transform.Find("Target").localScale = Vector2.Lerp(new Vector2(2f, 2f), new Vector2(0.278125f, 2f), (float)endFrames / 11f);
            base.transform.Find("Target").GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f - (float)endFrames / 11f);
            if (endFrames == 11)
            {
                Object.Destroy(base.gameObject);
            }
        }
    }
    
    public void PlayHitAnimation(EnemyBase enemy, int partyMember, float successRate, int barIndex)
    {
            PlayerAttackAnimation playerAttackAnimation = Object.Instantiate(Resources.Load<GameObject>("battle/Slice")).GetComponent<PlayerAttackAnimation>();
            playerAttackAnimation.AssignValues(enemy, partyMember, successRate, Object.FindObjectOfType<PartyPanels>().NumOfActivePartyMembers());

    }
    /*
    public void PlayMiniHitAnimation(EnemyBase enemy, int partyMember)
    {
        PlayerAttackAnimation playerAttackAnimation = null;
        if (Util.GameManager().GetWeapon(partyMember) == 32)
        {
            playerAttackAnimation = Object.Instantiate(Resources.Load<GameObject>("battle/SmallFist")).GetComponent<SmallFistAttack>();
        }
        if (playerAttackAnimation != null)
        {
            playerAttackAnimation.AssignValues(enemy, partyMember, 1f, Object.FindObjectOfType<PartyPanels>().NumOfActivePartyMembers());
        }
    }
    */
    public void SetEnemies(EnemyBase me)
    {
        enemies = new EnemyBase[1] {me};
    }

    public void SetAttackers(bool attack)
    {
        if (attack)
        {
            bars.AssignValues(enemies[0], 0);
        }
        return;
    }

    public EnemyBase[] GetEnemies()
    {
        return enemies;
    }
    public void EndedFaster()
    {
        ended = true;
    }
    public bool IsGoing()
    {
        return (!done || !FindFirstObjectByType<FightTargetBar>().IsDisabled() || !ended);
    }
}
