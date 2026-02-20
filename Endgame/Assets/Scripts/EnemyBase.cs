using UnityEngine;

public class OverworldEnemyBase : MonoBehaviour
{
    [SerializeField]
    protected int defeatFlagID = 10;

    [SerializeField]
    protected int counterFlagID = 11;

    [SerializeField]
    protected int killExhaustCount = 3;

    [SerializeField]
    protected int battleId;

    [SerializeField]
    protected GameObject npcRespawn;

    [SerializeField]
    protected int hardModeBattleID = -1;

    [SerializeField]
    protected GameObject hardModeNpcRespawn;

    [SerializeField]
    private bool instantSpareRespawn;

    protected GameObject respawned;

    protected Vector3 origPos;

    protected Vector3 dif;

    protected int frames;

    protected bool running;

    protected int detectionFrames;

    protected bool detecting;

    protected bool canDetectPlayer = true;

    private int battleFrames;

    protected bool initiateBattle;

    protected bool disabled;

    private bool handled;

    protected bool runFromPlayer;

    protected float speed = 4f;

    protected SpriteRenderer exclaim;

    protected SpriteRenderer sr;

    protected int sortingOrderOffset;

    protected Animator anim;

    protected Rigidbody2D rigidbody2D;

    protected virtual void Awake()
    {
        if ((bool)GetComponentInChildren<EnemyDetectionRange>())
        {
            GetComponentInChildren<EnemyDetectionRange>().SetParentEnemy(this);
        }
        exclaim = base.transform.Find("Exclaim").GetComponent<SpriteRenderer>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        origPos = base.transform.position;
        if ((int)Util.GameManager().GetFlag(108) == 1)
        {
            if (hardModeBattleID > -1)
            {
                battleId = hardModeBattleID;
            }
            if (hardModeNpcRespawn != null)
            {
                npcRespawn = hardModeNpcRespawn;
            }
        }
        bool flag = false;
        if ((GetCounter() >= killExhaustCount && GetDefeatStatus() != 2) || (GetDefeatStatus() == 2 && GetCounter() > 1))
        {
            Object.Destroy(base.gameObject);
            flag = true;
        }
        else if (GetDefeatStatus() == 2 && GetCounter() <= 1)
        {
            InstantSpareRespawn();
            Object.Destroy(base.gameObject);
            flag = true;
        }
        else if (GetCounter() == killExhaustCount - 1 && killExhaustCount > 1)
        {
            runFromPlayer = true;
        }
        if (counterFlagID > -1)
        {
            if ((int)FindFirstObjectByType<GameManager>().GetFlag(53) == 1 && counterFlagID < 53 && !flag)
            {
                Object.Destroy(base.gameObject);
            }
            else if ((int)Util.GameManager().GetFlag(150) != 0 && counterFlagID <= 148 && counterFlagID != 101)
            {
                Object.Destroy(base.gameObject);
            }
            else if ((int)Util.GameManager().GetFlag(116) != 0 && counterFlagID == 101 && (int)Util.GameManager().GetFlag(13) == 0)
            {
                Object.Destroy(base.gameObject);
            }
            else if ((int)Util.GameManager().GetFlag(210) == 1 && counterFlagID <= 202 && (int)Util.GameManager().GetFlag(87) >= 8)
            {
                Object.Destroy(base.gameObject);
            }
        }
    }

    protected virtual void Update()
    {
        if (!disabled)
        {
            if (!detecting)
            {
                exclaim.enabled = false;
            }
            if (!initiateBattle)
            {
                if (detecting)
                {
                    detectionFrames++;
                    if ((detectionFrames == 10 && runFromPlayer) || detectionFrames == 15)
                    {
                        StartRunning();
                    }
                }
                if (running)
                {
                    RunAlgorithm();
                }
            }
            else
            {
                battleFrames++;
                if (battleFrames == 1)
                {
                    GetComponents<AudioSource>()[0].Play();
                }
                if (battleFrames == 10)
                {
                    GetComponents<AudioSource>()[1].Play();
                }
                if (battleFrames == 30)
                {
                    FindFirstObjectByType<PlayerController>().InitiateBattle(battleId);
                    disabled = true;
                }
            }
            //sr.sortingOrder = Mathf.RoundToInt(base.transform.position.y * -5f) + sortingOrderOffset;
            //exclaim.sortingOrder = sr.sortingOrder;
        }
        else
        {
            sr.enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
            exclaim.enabled = false;
        }
    }

    protected virtual void LateUpdate()
    {
        if ((bool)rigidbody2D)
        {
            rigidbody2D.velocity = Vector2.zero;
        }
    }

    protected virtual void StartRunning()
    {
        detecting = false;
        running = true;
        exclaim.enabled = false;
    }

    protected Vector3 CalculateDifference(float speed)
    {
        return Vector3.MoveTowards(base.transform.position, FindFirstObjectByType<PlayerController>().transform.position, speed / 48f) - base.transform.position;
    }

    protected virtual void RunAlgorithm()
    {
        Vector3 vector = Vector3.MoveTowards(base.transform.position, FindFirstObjectByType<PlayerController>().transform.position, speed / 48f);
        dif = vector - base.transform.position;
        if (runFromPlayer)
        {
            vector = base.transform.position - dif;
        }
        if ((bool)rigidbody2D)
        {
            rigidbody2D.MovePosition(vector);
        }
    }

    public virtual void DetectPlayer()
    {
        if (!handled && !disabled && canDetectPlayer && !detecting && !running)
        {
            exclaim.enabled = true;
            exclaim.GetComponent<AudioSource>().Play();
            detecting = true;
        }
    }

    public void ActivateHandled()
    {
        handled = true;
        disabled = true;
    }

    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.GetComponent<PlayerController>() || disabled || !canDetectPlayer)
        {
            return;
        }
        if ((bool)FindFirstObjectByType<SAVEPoint>() && FindFirstObjectByType<SAVEPoint>().IsSaving())
        {
            FindFirstObjectByType<SAVEPoint>().CancelSave();
        }
        UIComponent[] array = FindObjectsOfType<UIComponent>();
        foreach (UIComponent uIComponent in array)
        {
            if (!(uIComponent.GetType() == typeof(ActionPartyPanels)))
            {
                uIComponent.CancelControlReturn();
                Object.Destroy(uIComponent.gameObject);
            }
        }
        SelectableUIComponent[] array2 = Object.FindObjectsOfType<SelectableUIComponent>();
        foreach (SelectableUIComponent obj in array2)
        {
            obj.CancelControlReturn();
            Object.Destroy(obj.gameObject);
        }
        initiateBattle = true;
        FindFirstObjectByType<GameManager>().DisablePlayerMovement(deactivatePartyMembers: false);
        OverworldEnemyBase[] array3 = Object.FindObjectsOfType<OverworldEnemyBase>();
        foreach (OverworldEnemyBase overworldEnemyBase in array3)
        {
            if (overworldEnemyBase != this)
            {
                overworldEnemyBase.StopRunning();
            }
        }
    }

    public virtual void InstantSpareRespawn()
    {
        if ((bool)npcRespawn)
        {
            respawned = Object.Instantiate(npcRespawn, base.transform.parent);
        }
    }

    public virtual void StopRunning()
    {
        exclaim.enabled = false;
        canDetectPlayer = false;
        detecting = false;
        detectionFrames = 0;
        running = false;
        if ((bool)GetComponentInChildren<EnemyDetectionRange>())
        {
            GetComponentInChildren<EnemyDetectionRange>().DeactivateTrigger();
        }
    }

    public void ForceDisable()
    {
        disabled = true;
    }

    public void Reactivate()
    {
        canDetectPlayer = true;
        if ((bool)GetComponentInChildren<EnemyDetectionRange>())
        {
            GetComponentInChildren<EnemyDetectionRange>().ActivateTrigger();
        }
    }

    public int GetDefeatStatus()
    {
        return (int)FindFirstObjectByType<GameManager>().GetFlag(defeatFlagID);
    }

    public int GetCounter()
    {
        if (counterFlagID <= -1)
        {
            if (GetDefeatStatus() == 0)
            {
                return 0;
            }
            return 1;
        }
        return (int)FindFirstObjectByType<GameManager>().GetFlag(counterFlagID);
    }

    public int GetDefeatFlagID()
    {
        return defeatFlagID;
    }

    public int GetCounterFlagID()
    {
        return counterFlagID;
    }

    public int GetBattleID()
    {
        return battleId;
    }

    public int GetKillExhaustCount()
    {
        return killExhaustCount;
    }

    public bool IsDisabled()
    {
        return disabled;
    }

    public bool CanInstantlyRespawn()
    {
        return instantSpareRespawn;
    }

    public bool IsHandled()
    {
        return handled;
    }
}
