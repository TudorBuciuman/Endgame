using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private int posDistance = 10;

    [SerializeField]
    private Vector3 posOffset = Vector3.zero;
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

    private bool locked;

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

    protected bool isPlayer = true;

    private void Awake()
    {
        gm = FindFirstObjectByType<GameManager>();
        sr = base.transform.GetComponent<SpriteRenderer>();
        anim = base.transform.GetComponent<Animator>();
        anim.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("kris_ow");
        if (anim.runtimeAnimatorController == null)
            Application.Quit();
        col = base.transform.GetComponent<BoxCollider2D>();
        col.offset = new Vector2(0f, -0.55f);
        col.size = new Vector2(0.8f, 0.4f);
        rigid2D = base.transform.GetComponent<Rigidbody2D>();
        rigid2D.bodyType = RigidbodyType2D.Dynamic;
        rigid2D.gravityScale = 0f;
        rigid2D.freezeRotation = true;
        spd = 6f;
        canMove = true;
        initiating = false;
        iFrame = 0;
        iFrameMax = 0;
        moveFrames = 0;
        animControl = true;
        sr.enabled = true;
        SetCollision(onoff: true);
        aud = GetComponents<AudioSource>();
    }


    private void Update()
    {
        if (animControl)
        {
            anim.SetFloat("speed", 0.75f);
        }
        if (!canMove && animControl)
        {
            spd = 6f;
            runTimer = 0;
            anim.SetBool("isMoving", value: false);
        }
        else if ((HoldingMoveButtons() || sliding) && gm.canMove)
        {
            rigid2D.constraints = RigidbodyConstraints2D.FreezeRotation;
            HandleRun();
            spd = 6f;
        }
        else if (animControl)
        {
            spd = 6f;
            anim.SetBool("isMoving", value: false);
            rigid2D.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        //sr.sortingOrder = Mathf.RoundToInt(base.transform.position.y * -5f);
        if (initiating)
        {
            canMove = false;
            iFrame++;
            if (iFrame == 1 || iFrame == 5 || iFrame == 9)
            {
                gm.PlayGlobalSFX("sounds/snd_noise");
                soul.GetComponent<SpriteRenderer>().enabled = true;
            }
            if (iFrame == 3 || iFrame == 7)
            {
                soul.GetComponent<SpriteRenderer>().enabled = false;
            }
            if (iFrame == 11)
            {
                sr.enabled = false;
                gm.PlayGlobalSFX("sounds/snd_battlestart");
            }
            if (iFrame >= 11)
            {
                soul.transform.position = Vector3.Lerp(oldSoulPos, soulPos, ((float)iFrame - 11f) / (float)moveFrames);
            }
            if (iFrame > iFrameMax)
            {
                SetCollision(onoff: false);
                initiating = false;
                iFrame = 0;
                //gm.StartBattle(battleId);
            }
        }
        lastPos = transform.position;
    }
    private void HandleRun()
    {
        moveDir = new Vector3(UTInput.GetAxis("Horizontal"), UTInput.GetAxis("Vertical"));

        if (moveDir != Vector3.zero)
        {
            movePM = true;
            if (new List<Vector2>
        {
            Vector2.up,
            Vector2.left,
            Vector2.down,
            Vector2.right
        }.Contains(moveDir))
            {
                faceDir = moveDir;
            }
            else if (0f - moveDir.x == faceDir.x || 0f - moveDir.y == faceDir.y)
            {
                faceDir = new Vector3(0f, moveDir.y);
            }
            ChangeDirection(faceDir);
        }
        else
            movePM = false;

            rigid2D.MovePosition(base.transform.position + moveDir * spd * spdMultiplier / 48f + posEffect);

        
        if (animControl)
        {
            anim.SetBool("isMoving", ProperlyMovedLastFrame());
            if (ProperlyMoved())
            {
                anim.Play("walk");
            }
        }
    }
    public bool ProperlyMovedLastFrame()
    {
        if (!(Mathf.Round(Mathf.Abs(base.transform.position.x - lastPos.x) * 48f) > 1f))
        {
            return Mathf.Round(Mathf.Abs(base.transform.position.y - lastPos.y) * 48f) > 1f;
        }
        return true;
    }

    public bool ProperlyMoved()
    {
        if (!(Mathf.Round(Mathf.Abs(base.transform.position.x - moveLastPos.x) * 48f) > 1f))
        {
            return Mathf.Round(Mathf.Abs(base.transform.position.y - moveLastPos.y) * 48f) > 1f;
        }
        return true;
    }
    private bool HoldingMoveButtons()
    {
        if (Input.GetAxis("Horizontal") == 0f)
        {
            return Input.GetAxis("Vertical") != 0f;
        }
        return true;
    }

    public void ChangeDirection(Vector2 dir)
    {
        anim.SetFloat("dirX", dir[0]);
        anim.SetFloat("dirY", dir[1]);
    }
    public bool IsInitiatingBattle()
    {
        return initiating;
    }
    public void Lock()
    {
        locked = true;
    }

    public void Unlock()
    {
        locked = false;
    }

    public bool IsMoving()
    {
        return anim.GetBool("isMoving");
    }

    public void EnableStepSounds(string customFootStep = "")
    {
        this.customFootStep = customFootStep;
        usingStepSounds = true;
    }

    public void DisableStepSounds()
    {
        usingStepSounds = false;
    }

    public float GetSpeed()
    {
        return spd;
    }

    public void ToggleNoclip()
    {
        noclip = !noclip;
        col.enabled = !noclip;
    }

    public bool GetNoclip()
    {
        return noclip;
    }

    public void SetCollision(bool onoff)
    {
        GetComponentInChildren<InteractionTrigger>().GetComponent<BoxCollider2D>().enabled = onoff;
        if (!noclip)
        {
            col.enabled = onoff;
        }
    }
    public void SetSprite(Sprite sprite)
    {
        sr.sprite = sprite;
    }
    public void SetSprite(string spriteName)
    {
        sr.sprite = Resources.Load<Sprite>("player/Kris/" + spriteName);
    }
    public void SetPosEffect(Vector3 posEffect)
    {
        this.posEffect = posEffect;
    }

    public bool CanMove()
    {
        return canMove;
    }

    public void SetSelfAnimControl(bool setAnimControl)
    {
        animControl = setAnimControl;
    }

    public void EnableAnimator()
    {
        anim.enabled = true;
    }

    public void DisableAnimator()
    {
        anim.enabled = false;
    }

    public void PlayStepSound()
    {
        if (!usingStepSounds)
        {
            return;
        }
        if (customFootStep == "")
        {
            if (aud[footstep].clip == null || aud[footstep].clip.name != "snd_step" + (footstep + 1))
            {
                aud[footstep].clip = Resources.Load<AudioClip>("sounds/snd_step" + (footstep + 1));
            }
        }
        else
        {
            aud[footstep].clip = Resources.Load<AudioClip>(customFootStep);
        }
        aud[footstep].Play();
        footstep = (footstep + 1) % 2;
    }
    public void HandleSpawn(Vector3 spawnPos, Vector2 spawnDir)
    {
        base.transform.position = spawnPos;
        ChangeDirection(spawnDir);
    }
}
