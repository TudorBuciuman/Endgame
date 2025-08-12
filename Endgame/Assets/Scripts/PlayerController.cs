using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    private GameManager gm;

    private SpriteRenderer sr;

    private Animator anim;

    private BoxCollider2D col;

    private Rigidbody2D rigid2D;
    
    private float spd = 6;

    private bool canMove;

    private bool movePM;

    private Vector3 lastPos;

    private Vector3 moveLastPos;

    private Vector3 posEffect = Vector3.zero;

    private float spdMultiplier = 1f;

    private Vector2 faceDir = Vector2.down;

    private Vector3 moveDir = Vector2.zero;

    private int runTimer;

    private int battleId;

    private bool initiating;

    private int iFrame;

    private int iFrameMax;

    private int moveFrames;

    private GameObject soul;

    private Vector2 oldSoulPos;

    private Vector2 soulPos;

    private bool specialBattleFreeze;

    private string curSpriteName = "";

    private bool useCustomSprites;

    private bool sliding;

    private int slideFrames;

    private bool forceSendPositions;

    private bool animControl;

    public bool cellphoneCall;

    public bool noclip;

    private bool isFrisk;

    private bool canWallDance;

    private bool usingStepSounds;

    private string customFootStep = "";

    private AudioSource[] aud;

    private int footstep;

    private float stepEncCounter;

    private int moveState;

    private bool depressed;

    private bool autoRun;

    private bool useRunAnim = true;
    void Awake()
    {
        rigid2D = base.transform.GetComponent<Rigidbody2D>();
        rigid2D.bodyType = RigidbodyType2D.Dynamic;
        rigid2D.gravityScale = 0f;
        rigid2D.freezeRotation = true;
        spd = 4f;
    }
    private void Start()
    {
        gm = GameManager.instance;

    }
    // Update is called once per frame
    void Update()
    {
        rigid2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        if(gm.canMove)
        HandleMovement();

    }
    private bool HoldingMoveButtons()
    {
        if (Input.GetAxis("Horizontal") == 0f)
        {
            return Input.GetAxis("Vertical") != 0f;
        }
        return true;
    }

    private void HandleMovement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // Calculate movement direction based on input
        //Vector3 moveDir = new Vector3(horizontalInput, verticalInput).normalized;
        Vector3 moveDir = new Vector3(horizontalInput, verticalInput);
        /*
        if (sliding)
        {
            moveDir.y = -1.75f;
        }
        */
        // Check if moving
        bool isMoving = moveDir != Vector3.zero;

        if (isMoving)
        {
            movePM = true;
        }

        // Adjust movement speed (remove the run function)
        // Move the Rigidbody2D
        rigid2D.MovePosition(transform.position + moveDir * spd * Time.deltaTime + posEffect);
        /*
        // Adjust facing direction
        if (sliding)
        {
            faceDir = Vector2.down;
        }
        */
        if (moveDir != Vector3.zero)
        {
            faceDir = new Vector2(moveDir.x, moveDir.y);
        }

        //ChangeDirection(faceDir);

        // Update animation
        /*
        if (animControl)
        {
            anim.SetBool("isMoving", isMoving);

            if (isMoving)
            {
                string animationState = (SceneManager.GetActiveScene().buildIndex == 123) ? "runb" : "walk";
                anim.Play(animationState);
            }
        }
        /*
        // Step encounter logic (if applicable)
        if (!sliding && Object.FindObjectOfType<StepEncounterer>() != null && isMoving)
        {
            stepEncCounter += 0.1f * (CheckRun() ? 1.5f : 0.75f);

            if (stepEncCounter >= 1f)
            {
                stepEncCounter -= 1f;
                Object.FindObjectOfType<StepEncounterer>().AddStep();
            }
        }
        */
        //Move();
    }
    private void Move(Vector3 v)
    {
        base.transform.position = v;
        anim.Play("walk");
        //GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt((base.transform.position.y - posOffset.y) * -5f);

    }
    public void ChangeDirection(Vector2 dir)
    {
        anim.SetFloat("dirX", dir[0]);
        anim.SetFloat("dirY", dir[1]);
    }
}
