using UnityEngine;

public class OverworldLoox : OverworldEnemyBase
{
    [SerializeField]
    private Vector3[] positions = new Vector3[1] { Vector3.zero };

    private int goingToPos = 1;

    private bool hardMode;

    private bool runToNextPoint;

    protected override void Awake()
    {
        if ((int)Object.FindObjectOfType<GameManager>().GetFlag(108) == 1)
        {
            hardMode = true;
        }
        base.Awake();
        sortingOrderOffset = -2;
        if (positions.Length == 1)
        {
            goingToPos = 0;
        }
        base.transform.position = positions[0];
        speed = 10f;
        if (hardMode)
        {
            base.transform.GetChild(0).localPosition = new Vector3(0f, 0.97f);
            speed = 0f;
            //anim.Play("Astigmatism Idle");
        }
        else if (runFromPlayer)
        {
            speed = 5f;
            runFromPlayer = false;
        }
    }

    protected override void Update()
    {
        if (!disabled && ((!detecting && !running && !hardMode) || (hardMode && runToNextPoint)))
        {
            Vector3 position = base.transform.position;
            float num = (hardMode ? 12 : 3);
            base.transform.position = Vector3.MoveTowards(base.transform.position, positions[goingToPos], num / 48f);
            GetComponent<SpriteRenderer>().flipX = base.transform.position.x - position.x > 0f;
            if (base.transform.position == positions[goingToPos])
            {
                goingToPos++;
                if (goingToPos == positions.Length)
                {
                    goingToPos = 0;
                }
                if (hardMode)
                {
                    runToNextPoint = false;
                }
            }
        }
        if (initiateBattle)
        {
            //anim.SetFloat("speed", 0f);
        }
        base.Update();
    }

    public override void DetectPlayer()
    {
        base.DetectPlayer();
        //anim.SetFloat("speed", 0f);
        //if (hardMode)
        //{
            //anim.Play("Astigmatism Close", 0, 0f);
       // }
    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        runToNextPoint = false;
    }

    public override void StopRunning()
    {
        base.StopRunning();
        runToNextPoint = false;
    }

    protected override void RunAlgorithm()
    {
        if (!hardMode)
        {
            //anim.SetFloat("speed", 1.5f);
            base.RunAlgorithm();
            GetComponent<SpriteRenderer>().flipX = dif.x > 0f;
            return;
        }
        //anim.Play("Astigmatism Chase");
        if (runFromPlayer)
        {
            if (Vector3.Distance(base.transform.position, Object.FindObjectOfType<PlayerController>().transform.position) < 2f)
            {
                runToNextPoint = true;
            }
            return;
        }
        if (speed < 20f)
        {
            speed += 1f;
        }
        base.RunAlgorithm();
    }
}
