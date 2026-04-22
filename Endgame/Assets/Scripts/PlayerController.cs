using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

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
    
    private float spd = 3;
    private float runspd = 4;

    private bool canMove;
    private bool canRun=false;

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

    private bool usingStepSounds;

    private string customFootStep = "";

    private AudioSource[] aud;

    private int footstep;

    protected bool isPlayer = true;

    private int moveState;

    private int movementFrame;
    private bool activated;

    private bool doLastMove;
    public static PlayerController instance;

    private void Awake()
    {
        gm = FindFirstObjectByType<GameManager>();
        sr = base.transform.GetComponent<SpriteRenderer>();
        anim = base.transform.GetComponent<Animator>();
        anim.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("pawn_ow");
        if (anim.runtimeAnimatorController == null)
            Application.Quit();
        col = base.transform.GetComponent<BoxCollider2D>();
        col.offset = new Vector2(0f, -0.55f);
        col.size = new Vector2(0.8f, 0.4f);
        rigid2D = base.transform.GetComponent<Rigidbody2D>();
        rigid2D.bodyType = RigidbodyType2D.Dynamic;
        rigid2D.gravityScale = 0f;
        rigid2D.freezeRotation = true;
        spd = 3f;
        runspd = 4;
        canMove = true;
        initiating = false;
        iFrame = 0;
        iFrameMax = 0;
        moveFrames = 0;
        movementFrame = 0;
        animControl = true;
        sr.enabled = true;
        SetCollision(onoff: true);
        aud = GetComponents<AudioSource>();

        
    }
    private void Start()
    {
        canRun = GameManager.test;
    }


    private void FixedUpdate()
    {
        if (!locked)
        {
            if (animControl)
            {
                anim.SetFloat("speed", 0.75f);
            }
            if (!canMove && animControl)
            {
                anim.SetBool("isMoving", value: false);
            }
            else if ((HoldingMoveButtons() || sliding) && gm.canMove)
            {
                rigid2D.constraints = RigidbodyConstraints2D.FreezeRotation;
                HandleRun();
                if (movementFrame > 5000)
                    movementFrame = 10;
                movementFrame++;
            }
            else if (animControl)
            {
                anim.SetBool("isMoving", value: false);
                rigid2D.constraints = RigidbodyConstraints2D.FreezeAll;
            }
            if (!HoldingMoveButtons())
            {
                movementFrame = 0;
            }
            lastPos = transform.position;
        }
        if (initiating)
        {
            canMove = false;
            iFrame++;
            if (iFrame == 1 || iFrame == 7 || iFrame == 13)
            {
                gm.PlayGlobalSFX("sounds/snd_noise");
                soul.GetComponent<SpriteRenderer>().enabled = true;
            }
            if (iFrame == 5 || iFrame == 9)
            {
                soul.GetComponent<SpriteRenderer>().enabled = false;
            }
            if (iFrame == 15)
            {
                sr.enabled = false;
                gm.PlayGlobalSFX("sounds/snd_battlestart");
            }
            if (iFrame >= 15)
            {
                soul.transform.position = Vector3.Lerp(oldSoulPos, soulPos, ((float)iFrame - 15f) / (float)moveFrames);
            }
            if (iFrame > iFrameMax)
            {
                SetCollision(onoff: false);
                initiating = false;
                iFrame = 0;
                gm.StartBattle(battleId);
            }
        }
    }
    public Vector2 GetDirection()
    {
        return new Vector2(anim.GetFloat("dirX"), anim.GetFloat("dirY"));
    }
    private void HandleRun()
    {
        bool lastmoveTr = false;
        moveDir = new Vector3(UTInput.GetAxisRaw("Horizontal"), UTInput.GetAxisRaw("Vertical"));
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
        {
            if (movementFrame > 1)
            {
                movementFrame = 1;
                lastmoveTr = true;
            }
            else
            {
                movementFrame = 0;
                lastmoveTr = false;
            }
            movePM = false;
        }
        if (animControl)
        {
            anim.SetBool("isMoving", (ProperlyMovedLastFrame()) || lastmoveTr);
            if (ProperlyMoved())
            {
                anim.Play("walk");
                if (!canRun || !Input.GetKey(KeyCode.X))
                {
                    anim.SetFloat("speed", 0.75f);
                    if (movementFrame > 2)
                    {
                        rigid2D.MovePosition(transform.position + moveDir * spd * spdMultiplier * Time.deltaTime);
                    }
                }
                else
                {
                    anim.SetFloat("speed", 0.85f);
                    if (movementFrame > 2)
                    {
                        rigid2D.MovePosition(transform.position + moveDir * runspd * spdMultiplier * Time.deltaTime);
                    }
                }
            }
            else
                movementFrame = 0;
        }
    }
    public bool ProperlyMovedLastFrame()
    {

        if (!((Mathf.Abs(base.transform.position.x - lastPos.x) * 101) > 1f))
        {
            return (Mathf.Abs(base.transform.position.y - lastPos.y) * 101) > 1f;
        }
        return true;
    }

    public bool ProperlyMoved()
    {
        return true;
        if (!((Mathf.Abs(base.transform.position.x - lastPos.x)*500) > 1f))
        {
            return (Mathf.Abs(base.transform.position.y - lastPos.y)*500) > 1f;
        }
        return true;
    }
    private bool HoldingMoveButtons()
    {
        if (UTInput.GetAxisRaw("Horizontal") == 0f)
        {
           return UTInput.GetAxisRaw("Vertical") != 0f;
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
    public void InitiateBattle(Vector2 toSoulPos, int frames)
    {
        if (!gm.GetPlayingMusic().Contains("core"))
        {
            gm.PauseMusic();
        }
        gm.DisablePlayerMovement(deactivatePartyMembers: false);
        GetComponentInChildren<InteractionTrigger>().GetComponent<BoxCollider2D>().enabled = false;
        SpriteRenderer[] componentsInChildren = GameObject.Find("MAP").GetComponentsInChildren<SpriteRenderer>();
        if(componentsInChildren!=null)
        for (int i = 0; i < componentsInChildren.Length; i++)
        {
            componentsInChildren[i].enabled = false;
        }
        Collider2D[] componentsInChildren2 = GameObject.Find("MAP").GetComponentsInChildren<Collider2D>();
        for (int i = 0; i < componentsInChildren2.Length; i++)
        {
            componentsInChildren2[i].enabled = false;
        }
        AudioSource[] componentsInChildren3 = GameObject.Find("MAP").GetComponentsInChildren<AudioSource>();
        foreach (AudioSource audioSource in componentsInChildren3)
        {
            if (audioSource.isPlaying)
            {
                audioSource.enabled = false;
            }
        }
        TilemapRenderer[] componentsInChildren4 = GameObject.Find("MAP").GetComponentsInChildren<TilemapRenderer>();
        for (int i = 0; i < componentsInChildren4.Length; i++)
        {
            componentsInChildren4[i].enabled = false;
        }
        SpriteMask[] componentsInChildren5 = GameObject.Find("MAP").GetComponentsInChildren<SpriteMask>();
        for (int i = 0; i < componentsInChildren5.Length; i++)
        {
            componentsInChildren5[i].enabled = false;
        }
        moveFrames = frames;
        soulPos = toSoulPos + new Vector2(Camera.main.transform.position.x, Camera.main.transform.position.y);
        iFrameMax = 15 + moveFrames;
        locked = true;
        soul = Instantiate(Resources.Load<GameObject>("overworld/OWSoul"), base.transform);
        oldSoulPos = soul.transform.position;
        soul.transform.localScale = new Vector2(0.5f, 0.5f);
        soul.GetComponent<SpriteRenderer>().sortingOrder = sr.sortingOrder + 300;
        soul.GetComponent<SpriteRenderer>().enabled = false;
        initiating = true;
        //Aici am ramas
    }

    public void InitiateBattle()
    {
        InitiateBattle(new Vector2(-5.646f, -4.48f), 9);
    }

    public void InitiateBattle(int btl)
    {
        battleId = btl;
        InitiateBattle();
        SetCustomSoulColor(btl);
    }

    public void InitiateBattle(int btl, Vector2 toSoulPos, int frames)
    {
        battleId = btl;
        InitiateBattle(toSoulPos, frames);
        SetCustomSoulColor(btl);
    }
    public void SetMovement(bool newMove)
    {
        if (moveState == 1)
        {
            if (!canMove && !newMove)
            {
                specialBattleFreeze = true;
            }
            else if (newMove && specialBattleFreeze)
            {
                specialBattleFreeze = false;
            }
            return;
        }
        if (!col.enabled && newMove)
        {
            SetCollision(onoff: true);
        }
        if (canMove && !newMove && IsMoving())
        {
            movePM = false;
        }
        else if (!canMove && newMove && HoldingMoveButtons())
        {
            movePM = true;
        }
        canMove = newMove;
    }
    private void SetCustomSoulColor(int bt)
    {
        if (bt != 53)
        {
            soul.GetComponent<SpriteRenderer>().color = Color.red;//SOUL.GetSOULColorByID(Util.GameManager().GetFlagInt(312));
        }
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
    public void Deactivate()
    {
        if (!locked)
        {
            activated = false;
            doLastMove = true;
        }
    }
    public void Activate()
    {
        activated = true;
        canMove = true;
        locked = false;
    }
}
