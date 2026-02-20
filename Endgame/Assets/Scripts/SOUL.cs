using UnityEngine;

public class SOUL : MonoBehaviour
{
    public enum SoulMode
    {
        Normal = 0,
        Jump = 1,
        Shield = 2,
        Fall = 3,
        Slow = 4,
        Shoot = 5,
        Trap = 6
    }

    private static readonly Color[] SOUL_COLORS = new Color[7]
    {
        Color.red,
        new Color32(0, 60, byte.MaxValue, byte.MaxValue),
        new Color32(0, 192, 0, byte.MaxValue),
        new Color32(252, 166, 0, byte.MaxValue),
        new Color32(66, 252, byte.MaxValue, byte.MaxValue),
        new Color(1f, 1f, 0f),
        new Color32(213, 53, 217, byte.MaxValue)
    };

    private static readonly Color[] PASTEL_SOUL_COLORS = new Color[7]
    {
        new Color32(byte.MaxValue, 102, 102, byte.MaxValue),
        new Color32(102, 138, byte.MaxValue, byte.MaxValue),
        new Color32(96, 191, 96, byte.MaxValue),
        new Color32(252, 202, 101, byte.MaxValue),
        new Color32(153, byte.MaxValue, byte.MaxValue, byte.MaxValue),
        new Color32(byte.MaxValue, byte.MaxValue, 166, byte.MaxValue),
        new Color32(215, 139, 217, byte.MaxValue)
    };

    private Color soulColor;

    private bool isPlayer;

    [SerializeField]
    private bool inControl;

    private bool isMonster;

    private float maxSpd;

    private int frames;

    private int invFrames;

    private bool hurt;

    private bool shattered;

    private bool isGrabbed;

    private bool shot;

    private bool isMoving;

    private Vector3 prevPos = Vector3.zero;

    private SoulMode soulMode;

    private float blueGravity;

    private bool hitGround;

    private Vector2 gravityDir = Vector2.down;

    private Platform curPlatform;

    private readonly Vector3 unusedPos = new Vector3(5000f, 5000f, 5000f);

    private Vector3 lastPlatformPos;

    private bool slamming;

    private float slamPosA;

    private float slamPosB;

    private int slamFrames;

    private float slamMaxFrames;

    private bool emanating;

    private bool emnDoReverse;

    private SpriteRenderer sr;

    private AudioSource aud;

    private AudioSource aud2;

    private Rigidbody2D rigid2D;

    private bool debugInv;

    private SOULHitBox hitbox;

    private SOULGraze grazer;

    private SOULParryHitBox parryHitbox;

    private SOULDrainGraphic drainGraphic;

    private Vector3 pullForce = Vector3.zero;

    private SpriteRenderer lightShield;

    private bool lightShieldActivated;

    private int lightShieldHP;

    private bool lightShieldDeath;

    private int lightShieldDeathFrames;

    private bool parrying;

    private int parryFrames = 30;

    private bool parryDuringIframes;

    private bool parryHoldThisFrame;

    private bool dashing;

    private bool dashUsed;

    private bool wasGroundDash;

    private int dashFrames = 30;

    private bool dashHoldThisFrame;

    private Vector3 dashDir = Vector3.zero;

    private Vector3 dashspdreduction = Vector3.zero;

    private Vector3 lastValiddashDir = Vector3.zero;

    private bool dropThrough;

    private int bigShotCharge;

    private int bigShotCheating;

    private bool yDashEnabled;

    private bool yDashActive;

    private bool yDashing;

    private int yDashFrames;

    private Vector3 yDashDir = Vector3.up;

    private bool yDashBuffer;

    private BulletBase lastHitBullet;

    private bool papCharmHit;

    private float damageMulti = 1f;

    private void Awake()
    {
        shattered = false;
        frames = 0;
        invFrames = 30;
        isPlayer = false;
        maxSpd = 4f;
        soulMode = SoulMode.Normal;
        hitGround = false;
        blueGravity = 0f;
        emanating = false;
        emnDoReverse = false;
        isGrabbed = false;
        debugInv = false;
        sr = base.gameObject.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 100;
        sr.sprite = Resources.Load<Sprite>("battle/spr_soul");
        aud = base.gameObject.AddComponent<AudioSource>();
        aud2 = base.gameObject.AddComponent<AudioSource>();
        rigid2D = base.gameObject.AddComponent<Rigidbody2D>();
        rigid2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rigid2D.sleepMode = RigidbodySleepMode2D.NeverSleep;
        rigid2D.gravityScale = 0f;
        rigid2D.freezeRotation = true;
        base.gameObject.AddComponent<BoxCollider2D>();
        base.gameObject.layer = 2;
        hitbox = new GameObject("Hitbox", typeof(SOULHitBox), typeof(SpriteRenderer), typeof(BoxCollider2D)).GetComponent<SOULHitBox>();
        hitbox.SetParentSOUL(this);
        hitbox.transform.parent = base.transform;
        hitbox.transform.localPosition = Vector3.zero;
        grazer = new GameObject("Grazer", typeof(SOULGraze), typeof(SpriteRenderer), typeof(CircleCollider2D), typeof(AudioSource)).GetComponent<SOULGraze>();
        grazer.SetParentSOUL(this);
        grazer.transform.parent = base.transform;
        grazer.transform.localPosition = Vector3.zero;
        lightShield = new GameObject("LightShield", typeof(SpriteRenderer)).GetComponent<SpriteRenderer>();
        lightShield.transform.parent = base.transform;
        lightShield.sprite = Resources.Load<Sprite>("battle/spr_soul_lightshield");
        lastPlatformPos = unusedPos;
    }

    private void Update()
    {
        float num = maxSpd / (float)((!UTInput.GetButton("X") && soulMode != SoulMode.Slow) ? 1 : 2);
        if (isPlayer && !shattered)
        {
            if (soulMode == SoulMode.Jump && isGrabbed)
            {
                HandleDash();
            }
            if (soulMode == SoulMode.Normal || soulMode == SoulMode.Shoot)
            {
                HandleYDash();
            }
            if ((soulMode == SoulMode.Jump || soulMode == SoulMode.Fall) && !isGrabbed)
            {
                dashspdreduction = dashDir;
                if (soulMode == SoulMode.Jump)
                {
                    HandleDash();
                    if ((dashFrames < 30 && dashUsed) || (dashFrames < 12 && !dashUsed))
                    {
                        dashFrames++;
                        if (dashFrames == 12)
                        {
                            dashDir = Vector3.zero;
                        }
                        if (dashFrames >= 6)
                        {
                            int num2 = dashFrames - 6;
                            if (num2 == 0 && !hitGround)
                            {
                                dashspdreduction = Vector3.zero;
                                blueGravity = (GravityIsVertical() ? (dashDir.y * (0f - gravityDir.y)) : (dashDir.x * (0f - gravityDir.x))) * 20f;
                                MonoBehaviour.print(blueGravity);
                            }
                            else if (num2 > 0)
                            {
                                dashspdreduction = dashDir / num2;
                            }
                        }
                        if (dashFrames == 6)
                        {
                            dropThrough = false;
                            dashing = false;
                        }
                        if (dashFrames <= 6)
                        {
                            DashEffect component = Object.Instantiate(Resources.Load<GameObject>("vfx/DashEffect"), base.transform.position, Quaternion.identity).GetComponent<DashEffect>();
                            component.transform.eulerAngles = base.transform.eulerAngles;
                            component.SetAttributes(soulColor, sr);
                        }
                        if (dashFrames == 3)
                        {
                            sr.color = (dashUsed ? ((Color)new Color32(32, 32, 127, byte.MaxValue)) : soulColor);
                        }
                        else if (dashFrames <= 2)
                        {
                            sr.color = Color.white;
                        }
                        if (dashUsed && wasGroundDash)
                        {
                            if (dashFrames >= 3)
                            {
                                drainGraphic.SetGraphicType(Mathf.RoundToInt((float)(dashFrames - 3) / 1.5f), soulColor);
                            }
                            if (dashFrames == 30)
                            {
                                ResetDash();
                            }
                        }
                    }
                }
                if (!dropThrough)
                {
                    PlatformDetection();
                }
                Vector3 vector = Vector3.zero;
                if (curPlatform != null && lastPlatformPos != unusedPos)
                {
                    vector = curPlatform.transform.position - lastPlatformPos;
                }
                if (vector.y != 0f && pullForce.y != 0f)
                {
                    vector.y = 0f;
                }
                if (IsJumping() && inControl && hitGround && !slamming)
                {
                    base.transform.parent = null;
                    curPlatform = null;
                    blueGravity = 6.5f;
                    hitGround = false;
                    lastPlatformPos = unusedPos;
                    if (dashUsed && wasGroundDash)
                    {
                        ResetDash();
                    }
                }
                else if (!IsJumping() && inControl && blueGravity > 0f && !hitGround && !slamming)
                {
                    blueGravity = 0f;
                }
                else if (!hitGround && blueGravity > -7f && !slamming && inControl && !dashing)
                {
                    blueGravity -= 0.295f;
                }
                int num3 = 1572;
                num3 = ~num3;
                RaycastHit2D hit = Physics2D.Raycast(base.transform.position, -gravityDir, float.PositiveInfinity, num3);
                if ((hit.collider != null && HitCeil(hit) && blueGravity > 0f) || dashing)
                {
                    blueGravity = 0f;
                }
                if (hitGround && dashDir.y < 0f)
                {
                    dashDir.y = 0f;
                    dashspdreduction.y = 0f;
                }
                int num4 = ((gravityDir == Vector2.down || gravityDir == Vector2.left) ? 1 : (-1));
                if (inControl)
                {
                    if (GravityIsVertical())
                    {
                        rigid2D.MovePosition(base.transform.position + new Vector3(num * (Mathf.Round(UTInput.GetAxisRaw("Horizontal")) / 48f), blueGravity * (float)num4 / 48f) + vector + dashspdreduction + pullForce);
                    }
                    else
                    {
                        rigid2D.MovePosition(base.transform.position + new Vector3(blueGravity * (float)num4 / 48f, num * (Mathf.Round(UTInput.GetAxisRaw("Vertical")) / 48f)) + vector + dashspdreduction + pullForce);
                    }
                }
                else if (GravityIsVertical())
                {
                    rigid2D.MovePosition(base.transform.position + new Vector3(0f, blueGravity * (float)num4 / 48f) + vector + pullForce);
                }
                else
                {
                    rigid2D.MovePosition(base.transform.position + new Vector3(blueGravity * (float)num4 / 48f, 0f) + vector + pullForce);
                }
                if (curPlatform == null)
                {
                    lastPlatformPos = unusedPos;
                    Vector3 vector2;
                    Vector3 vector3;
                    if (GravityIsVertical())
                    {
                        vector2 = new Vector3(-0.125f, 0f);
                        vector3 = new Vector3(0.125f, 0f);
                    }
                    else
                    {
                        vector2 = new Vector3(0f, -0.125f);
                        vector3 = new Vector3(0f, 0.125f);
                    }
                    RaycastHit2D hit2 = Physics2D.Raycast(base.transform.position + vector2, gravityDir, float.PositiveInfinity, num3);
                    RaycastHit2D hit3 = Physics2D.Raycast(base.transform.position + vector3, gravityDir, float.PositiveInfinity, num3);
                    RaycastHit2D hit4 = Physics2D.Raycast(base.transform.position, gravityDir, float.PositiveInfinity, num3);
                    if (hit4.collider != null)
                    {
                        if (!HitFloor(hit4) && !HitFloor(hit2) && !HitFloor(hit3))
                        {
                            hitGround = false;
                        }
                        else if ((!hit4.collider.isTrigger && !hit4.transform.gameObject.tag.Contains("Bullet") && HitFloor(hit4)) || (!hit2.collider.isTrigger && !hit2.transform.gameObject.tag.Contains("Bullet") && HitFloor(hit2)) || (!hit3.collider.isTrigger && !hit3.transform.gameObject.tag.Contains("Bullet") && HitFloor(hit3)))
                        {
                            if (soulMode == SoulMode.Jump)
                            {
                                if (!hitGround)
                                {
                                    ResetDash();
                                }
                                hitGround = true;
                                blueGravity = 0f;
                            }
                            else if (soulMode == SoulMode.Fall)
                            {
                                SetGravityDirection(gravityDir * -1f);
                            }
                        }
                    }
                    else
                    {
                        hitGround = false;
                    }
                }
                else
                {
                    lastPlatformPos = curPlatform.transform.position;
                }
            }
            else if (soulMode == SoulMode.Normal || soulMode == SoulMode.Slow || soulMode == SoulMode.Shoot)
            {
                Vector3 vector4 = new Vector3(num * (Mathf.Round(UTInput.GetAxisRaw("Horizontal")) / 48f), num * (Mathf.Round(UTInput.GetAxisRaw("Vertical")) / 48f));
                if (!inControl)
                {
                    vector4 = Vector3.zero;
                }
                if (yDashActive)
                {
                    if (yDashing)
                    {
                        vector4 = yDashDir;
                    }
                    if (yDashFrames < 4)
                    {
                        Object.Instantiate(Resources.Load<GameObject>("vfx/DashEffect"), base.transform.position, Quaternion.identity).GetComponent<DashEffect>().SetAttributes(soulColor, sr);
                    }
                    yDashFrames++;
                    if (bigShotCharge < 20)
                    {
                        sr.color = Color.Lerp(Color.white, soulColor, (float)(yDashFrames - 2) / 6f);
                    }
                    if (yDashFrames < 4 && !UTInput.GetButton("X"))
                    {
                        yDashFrames = 4;
                    }
                    if (yDashFrames == 4)
                    {
                        yDashing = false;
                    }
                    if (yDashFrames == 7)
                    {
                        hitbox.GetComponent<Collider2D>().enabled = true;
                    }
                    if (yDashFrames == 8)
                    {
                        yDashActive = false;
                    }
                }
                rigid2D.MovePosition(base.transform.position + vector4 + pullForce);
            }
            Vector3 vector5 = (base.transform.position - prevPos) * 48f;
            if (Mathf.FloorToInt(Mathf.Abs(vector5.x)) != 0 || Mathf.FloorToInt(Mathf.Abs(vector5.y)) != 0 || (!hitGround && soulMode == SoulMode.Jump))
            {
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }
            if (soulMode == SoulMode.Slow)
            {
                HandleParry();
                if (parryFrames < 30)
                {
                    parryFrames++;
                    if (parryFrames >= 3)
                    {
                        drainGraphic.SetGraphicType(Mathf.RoundToInt((float)(parryFrames - 3) / 1.6875f), soulColor);
                    }
                    if (parryFrames == 3)
                    {
                        sr.color = new Color32(68, 127, 127, byte.MaxValue);
                        parrying = false;
                        parryDuringIframes = false;
                    }
                    else if (parryFrames <= 2)
                    {
                        sr.color = Color.white;
                    }
                    else if (parryFrames == 30)
                    {
                        sr.color = soulColor;
                        drainGraphic.SetGraphicType(0, soulColor);
                    }
                }
            }
            /*
            else if (soulMode == SoulMode.Shoot)
            {
                if (bigShotDelay > 0)
                {
                    bigShotDelay--;
                }
                if (UTInput.GetButtonDown("Z") && bigShotDelay == 0 && Object.FindObjectsOfType<SOULShot>().Length < 4 && inControl)
                {
                    Object.Instantiate(Resources.Load<GameObject>("battle/SOULShot"), base.transform.position + new Vector3(0f, 0.3f), Quaternion.identity);
                }
                if (UTInput.GetButton("Z") && bigShotCharge < 20 && inControl)
                {
                    bigShotCharge++;
                    if (bigShotCharge == 5)
                    {
                        Object.Instantiate(Resources.Load<GameObject>("vfx/SOULShotChargeEffect"), base.transform).GetComponent<SOULShotChargeEffect>().Activate(this);
                    }
                    if (bigShotCharge < 15)
                    {
                        bigShotCheating = 0;
                    }
                    if (bigShotCharge == 20)
                    {
                        sr.color = Color.white;
                    }
                }
                if (UTInput.GetButtonUp("Z") && inControl)
                {
                    //if (bigShotCharge >= 5 && bigShotCharge < 20 && Object.FindObjectsOfType<SOULShot>().Length < 4)
                    {
                     //   Object.Instantiate(Resources.Load<GameObject>("battle/SOULShot"), base.transform.position + new Vector3(0f, 0.3f), Quaternion.identity);
                    }
                   // else if (bigShotCharge >= 20)
                    {
                     //   Object.Instantiate(Resources.Load<GameObject>("battle/BigShot"), base.transform.position + new Vector3(0f, 0.168f), Quaternion.identity);
                     //   bigShotCheating++;
                     //   bigShotDelay = 5;
                    //}
                }
                if (!UTInput.GetButton("Z") || !inControl)
                {
                    if ((bool)GetComponentInChildren<SOULShotChargeEffect>())
                    {
                        Object.Destroy(GetComponentInChildren<SOULShotChargeEffect>().gameObject);
                    }
                    if (!yDashActive)
                    {
                        sr.color = soulColor;
                    }
                    bigShotCharge = 0;
                }
            }*/
        }
            
        if (shattered)
        {
            frames++;
            if (frames == 39)
            {
                sr.enabled = false;
                for (int i = 0; i < 6; i++)
                {
                    GameObject obj = Object.Instantiate(Resources.Load<GameObject>("battle/SOULPiece"), base.gameObject.transform);
                    obj.transform.localPosition = new Vector2(Random.Range(-0.14f, 0.14f), Random.Range(-0.14f, 0.14f));
                    obj.GetComponent<SOULPiece>().ChangeSOULColor(soulColor);
                    obj.GetComponent<SOULPiece>().StartMoving();
                }
                aud.clip = Resources.Load<AudioClip>("sounds/snd_break2");
                aud.Play();
            }
        }
        if (hurt && !parrying)
        {
            frames++;
            if ((bool)GetComponentInChildren<SOULChargeEffect>() && soulMode == SoulMode.Slow && (bool)drainGraphic)
            {
                drainGraphic.SetGraphicType((30 - frames) / 2, Color.white);
            }
            if (frames % 6 == 0)
            {
                if (frames >= 6 && debugInv)
                {
                    frames = 0;
                }
                sr.color = soulColor;
            }
            else if (frames % 6 == 3)
            {
                if ((bool)GetComponentInChildren<SOULChargeEffect>() && soulMode == SoulMode.Slow)
                {
                    sr.color = soulColor;
                }
                else
                {
                    sr.color = new Color(soulColor.r / 2f, soulColor.g / 2f, soulColor.b / 2f);
                }
            }
            if (frames >= invFrames)
            {
                if ((bool)GetComponentInChildren<SOULChargeEffect>() && (bool)drainGraphic && soulMode == SoulMode.Slow)
                {
                    drainGraphic.SetGraphicType(0, Color.white);
                    Object.Destroy(GetComponentInChildren<SOULChargeEffect>().gameObject);
                }
                hurt = false;
                frames = 0;
                sr.color = soulColor;
            }
        }
        if (emanating)
        {
            if (frames % 6 == 0)
            {
                GameObject obj2 = new GameObject();
                obj2.transform.position = base.transform.position;
                obj2.AddComponent<EmanatingSOUL>().StartEffect(emnDoReverse, soulColor, isMonster);
            }
            frames++;
            if (frames > 18)
            {
                if (emnDoReverse)
                {
                    inControl = false;
                    sr.enabled = false;
                }
                emanating = false;
                emnDoReverse = false;
            }
        }
        prevPos = base.transform.position;
    }

    private void LateUpdate()
    {
        float num = maxSpd / (float)((!UTInput.GetButton("X")) ? 1 : 2);
        if (slamming && (soulMode == SoulMode.Jump || soulMode == SoulMode.Fall))
        {
            slamFrames++;
            if (GravityIsVertical())
            {
                rigid2D.MovePosition(new Vector3(base.transform.position.x + num * (Mathf.Round(UTInput.GetAxisRaw("Horizontal")) / 48f), Mathf.Lerp(slamPosA, slamPosB, (float)slamFrames / slamMaxFrames), base.transform.position.z));
            }
            else
            {
                rigid2D.MovePosition(new Vector3(Mathf.Lerp(slamPosA, slamPosB, (float)slamFrames / slamMaxFrames), base.transform.position.y + num * (Mathf.Round(UTInput.GetAxisRaw("Vertical")) / 48f), base.transform.position.z));
            }
            if ((float)slamFrames >= slamMaxFrames)
            {
                slamming = false;
                aud.clip = Resources.Load<AudioClip>("sounds/snd_hurt");
                aud.Play();
                aud2.clip = Resources.Load<AudioClip>("sounds/snd_crash");
                aud2.Play();
                FindFirstObjectByType<BattleCamera>().BlastShake();
            }
        }
        if (lightShieldActivated)
        {
            lightShield.sortingOrder = sr.sortingOrder;
            lightShield.enabled = sr.enabled;
        }
        else if (lightShieldDeath)
        {
            lightShieldDeathFrames++;
            lightShield.color = Color.Lerp(Color.white, new Color(1f, 1f, 1f, 0f), (float)lightShieldDeathFrames / 15f);
            lightShield.transform.localScale = Vector3.Lerp(new Vector3(1f, 1f), new Vector3(4f, 4f), (float)lightShieldDeathFrames / 15f);
            if (lightShieldDeathFrames == 15)
            {
                lightShieldDeath = false;
            }
        }
        else if (!lightShieldActivated)
        {
            lightShield.enabled = false;
        }
        dashHoldThisFrame = false;
        parryHoldThisFrame = false;
    }

    private void PlatformDetection()
    {
        if (soulMode != SoulMode.Jump && soulMode != SoulMode.Fall)
        {
            return;
        }
        Vector3 vector;
        Vector3 vector2;
        if (GravityIsVertical())
        {
            vector = new Vector3(-0.1f, 0.1f * gravityDir.y);
            vector2 = new Vector3(0.1f, 0.1f * gravityDir.y);
        }
        else
        {
            vector = new Vector3(0.1f * gravityDir.x, -0.1f);
            vector2 = new Vector3(0.1f * gravityDir.x, 0.1f);
        }
        RaycastHit2D raycastHit2D = Physics2D.Raycast(base.transform.position + vector, gravityDir, float.PositiveInfinity, 1024);
        RaycastHit2D raycastHit2D2 = Physics2D.Raycast(base.transform.position + vector2, gravityDir, float.PositiveInfinity, 1024);
        float num = (GravityIsVertical() ? (dashspdreduction.y * (0f - gravityDir.y)) : (dashspdreduction.x * (0f - gravityDir.x))) * 48f;
        if (curPlatform == null && !hitGround && blueGravity + num < 0f)
        {
            float num2 = (1f - (blueGravity + num)) / 48f;
            if (num2 < 0f)
            {
                num2 = 0f;
            }
            Debug.DrawRay(base.transform.position + vector, gravityDir * num2, Color.green);
            Debug.DrawRay(base.transform.position + vector2, gravityDir * num2, Color.green);
            if (raycastHit2D.distance < num2 && raycastHit2D.collider != null)
            {
                curPlatform = raycastHit2D.collider.gameObject.GetComponent<Platform>();
            }
            else if (raycastHit2D2.distance < num2 && raycastHit2D2.collider != null)
            {
                curPlatform = raycastHit2D2.collider.gameObject.GetComponent<Platform>();
            }
            if (!(curPlatform != null))
            {
                return;
            }
            base.transform.parent = curPlatform.transform;
            if (GravityIsVertical())
            {
                base.transform.localPosition = new Vector3(base.transform.localPosition.x, 0.19792f * (0f - gravityDir.y));
            }
            base.transform.parent = null;
            if (soulMode == SoulMode.Jump)
            {
                if (!hitGround)
                {
                    ResetDash();
                }
                hitGround = true;
                blueGravity = 0f;
                FindFirstObjectByType<SOULGraze>().AddTPBuildup(curPlatform.GetTPGain());
                curPlatform.Landed();
            }
            else if (soulMode == SoulMode.Fall)
            {
                SetGravityDirection(gravityDir * -1f);
            }
        }
        else if (curPlatform != null && (!(raycastHit2D.collider != null) || !(curPlatform == raycastHit2D.collider.gameObject.GetComponent<Platform>())) && (!(raycastHit2D2.collider != null) || !(curPlatform == raycastHit2D2.collider.gameObject.GetComponent<Platform>())) && ((raycastHit2D.collider == null && raycastHit2D2.collider == null) || (raycastHit2D.collider != null && curPlatform != raycastHit2D.collider.gameObject.GetComponent<Platform>() && raycastHit2D2.collider != null && curPlatform != raycastHit2D2.collider.gameObject.GetComponent<Platform>())))
        {
            curPlatform = null;
            base.transform.parent = null;
            hitGround = false;
            if (soulMode == SoulMode.Jump)
            {
                blueGravity = 0f;
            }
            lastPlatformPos = unusedPos;
        }
    }

    private void ResetDash()
    {
        if (soulMode == SoulMode.Jump)
        {
            dashUsed = false;
            wasGroundDash = false;
            dropThrough = false;
            sr.color = soulColor;
            drainGraphic.SetGraphicType(0, soulColor);
        }
    }

    public void CreateSOUL(Color color, bool monster, bool player)
    {
        soulColor = color;
        isMonster = monster;
        isPlayer = player;
        if (player)
        {
            inControl = true;
        }
        sr.color = soulColor;
        sr.flipY = isMonster;
    }

    public void IncrementSpeed()
    {
        if (maxSpd < 8f)
        {
            maxSpd += 1f;
        }
    }

    public float GetMaxSpeed()
    {
        return maxSpd;
    }

    public void SetPullForce(Vector3 pullForce)
    {
        this.pullForce = pullForce;
        if (!hitGround)
        {
            blueGravity -= pullForce.y;
        }
    }

    public void SetControllable(bool boo)
    {
        inControl = boo;
        isGrabbed = false;
        ResetDash();
    }

    public void SetFrozen(bool boo)
    {
        inControl = !boo;
        isGrabbed = boo;
    }

    public void Break()
    {
        sr.enabled = true;
        hurt = false;
        emanating = false;
        frames = 0;
        shattered = true;
        sr.sprite = Resources.Load<Sprite>("battle/spr_soulshatter");
        aud.clip = Resources.Load<AudioClip>("sounds/snd_break1");
        aud.Play();
    }

    public void Heal(int hp)
    {
        aud.clip = Resources.Load<AudioClip>("sounds/snd_heal");
        aud.Play();
        FindFirstObjectByType<GameManager>().Heal(hp);
    }

    public void HandleDash()
    {
        Vector3 vector = new Vector3(UTInput.GetAxisRaw("Horizontal"), UTInput.GetAxisRaw("Vertical"));
        if (vector == Vector3.zero || (vector == (Vector3)gravityDir && hitGround && !curPlatform))
        {
            return;
        }
        lastValiddashDir = vector;
        if (!inControl && isGrabbed && (bool)FindFirstObjectByType<JumpDashTutorial>() && UTInput.GetButtonDown("Z") && UTInput.GetAxisRaw("Vertical") > 0f)
        {
            SetFrozen(boo: false);
            FindFirstObjectByType<JumpDashTutorial>().Unfreeze();
        }
        if (soulMode != SoulMode.Jump || !inControl || dashing || !UTInput.GetButtonDown("Z") || dashHoldThisFrame || !(lastValiddashDir != Vector3.zero) || dashUsed)
        {
            return;
        }
        if ((bool)FindFirstObjectByType<JumpDashTutorial>())
        {
            Object.Destroy(FindFirstObjectByType<JumpDashTutorial>().gameObject);
        }
        aud.clip = Resources.Load<AudioClip>("sounds/snd_bomb");
        aud.Play();
        dashing = true;
        dashUsed = true;
        wasGroundDash = hitGround;
        dashFrames = 0;
        dashHoldThisFrame = true;
        float num = 4f;
        dashDir = lastValiddashDir;
        bool flag = ((!GravityIsVertical()) ? (dashDir.x != 0f && Mathf.Sign(dashDir.x) == Mathf.Sign(gravityDir.x)) : (dashDir.y != 0f && Mathf.Sign(dashDir.y) == Mathf.Sign(gravityDir.y)));
        bool flag2 = ((!GravityIsVertical()) ? (dashDir.x != 0f && Mathf.Sign(dashDir.x) != Mathf.Sign(gravityDir.x)) : (dashDir.y != 0f && Mathf.Sign(dashDir.y) != Mathf.Sign(gravityDir.y)));
        if (hitGround && flag)
        {
            if ((bool)curPlatform)
            {
                curPlatform = null;
                base.transform.parent = null;
                hitGround = false;
                dropThrough = true;
                if (GravityIsVertical())
                {
                    dashDir.y /= 2f;
                }
                else
                {
                    dashDir.x /= 2f;
                }
            }
            else if (GravityIsVertical())
            {
                dashDir.y = 0f;
            }
            else
            {
                dashDir.x = 0f;
            }
        }
        else if (hitGround && flag2)
        {
            curPlatform = null;
            base.transform.parent = null;
            hitGround = false;
        }
        if (dashDir.x != 0f && dashDir.y != 0f)
        {
            dashDir *= 0.9f;
        }
        dashDir /= num;
        Object.Instantiate(Resources.Load<GameObject>("vfx/SuccessfulDash"), base.transform.position, base.transform.rotation).GetComponent<SpriteRenderer>().sortingOrder = sr.sortingOrder - 20;
    }

    public void HandleYDash()
    {
        if (!inControl || !yDashEnabled || yDashActive || (!UTInput.GetButtonDown("X") && !yDashBuffer))
        {
            return;
        }
        yDashBuffer = false;
        Vector3 vector = new Vector3(UTInput.GetAxisRaw("Horizontal"), UTInput.GetAxisRaw("Vertical"));
        if (vector == Vector3.zero)
        {
            if (UTInput.GetButtonDown("X"))
            {
                yDashBuffer = true;
            }
            return;
        }
        sr.color = Color.white;
        aud.clip = Resources.Load<AudioClip>("sounds/snd_shootdash");
        aud.Play();
        yDashDir = vector * maxSpd * 3f / 48f;
        hitbox.GetComponent<Collider2D>().enabled = false;
        yDashing = true;
        yDashActive = true;
        yDashFrames = 0;
    }

    public void HandleParry()
    {
        if (soulMode == SoulMode.Slow && (!hurt || (hurt && parryDuringIframes)) && !parrying && UTInput.GetButtonDown("Z") && parryFrames >= 30 && inControl && !parryHoldThisFrame)
        {
            aud.clip = Resources.Load<AudioClip>("sounds/snd_bell_bounce_short");
            aud.Play();
            parrying = true;
            parryFrames = 0;
            parryHoldThisFrame = true;
        }
    }

    public void HandleParryCollision(BulletBase bullet = null)
    {
        if (bullet != null)
        {
            lastHitBullet = bullet;
        }
        if (parrying && (!hurt || (hurt && parryDuringIframes)))
        {
            MonoBehaviour.print("Parry Calc");
            int num = 5;
            if (hurt)
            {
                num -= Mathf.Abs(frames - 30) / 10;
            }
            MonoBehaviour.print("TP GAIN: " + num);
            if (num > 0)
            {
                //Object.FindObjectOfType<TPBar>().AddTP(num);
            }
            if (hurt)
            {
                frames -= 30;
            }
            else
            {
                frames = 0;
            }
            if (frames < -30)
            {
                frames = -30;
            }
            hurt = true;
            parrying = false;
            parryFrames = 30;
            parryDuringIframes = true;
            Object.Instantiate(Resources.Load<GameObject>("vfx/SuccessfulParry"), base.transform.position, Quaternion.identity).GetComponent<SpriteRenderer>().sortingOrder = sr.sortingOrder - 20;
            if (!GetComponentInChildren<SOULChargeEffect>())
            {
                SOULChargeEffect component = Object.Instantiate(Resources.Load<GameObject>("vfx/SOULChargeEffect"), base.transform).GetComponent<SOULChargeEffect>();
                component.transform.localPosition = Vector3.zero;
                component.Activate(this);
            }
            if ((bool)lastHitBullet)
            {
                lastHitBullet.Parry();
            }
        }
    }

    public void Damage(int hp)
    {
        HandleParry();
        if (parrying && (!hurt || (hurt && parryDuringIframes)))
        {
            HandleParryCollision();
        }
        else
        {
            if (hurt)
            {
                return;
            }
            parryDuringIframes = false;
            emanating = false;
            frames = 0;
            hurt = true;
            aud.clip = Resources.Load<AudioClip>("sounds/snd_hurt");
            aud.Play();
            if (FindFirstObjectByType<BattleCamera>() != null && invFrames > 0)
            {
                FindFirstObjectByType<BattleCamera>().HurtShake();
            }
            GameManager gameManager = Util.GameManager();
            PartyPanels partyPanels = FindFirstObjectByType<PartyPanels>();
            gameManager.Damage(hp);
            partyPanels.UpdateHP(gameManager.GetHP());
            if (lightShieldActivated)
            {
                lightShieldHP--;
                if (lightShieldHP == 0)
                {
                    lightShieldActivated = false;
                    lightShieldDeath = true;
                    lightShield.transform.parent = null;
                    lightShieldDeathFrames = 0;
                    gameManager.PlayGlobalSFX("sounds/snd_stardrop");
                }
            }
        }
    }

    public void Emanate(bool playSound)
    {
        if (!emanating && !hurt && !shattered)
        {
            frames = 0;
            emanating = true;
            sr.enabled = true;
            if (playSound)
            {
                aud.clip = Resources.Load<AudioClip>("sounds/snd_great_shine");
                aud.Play();
            }
        }
    }

    public void ActivateLightShield()
    {
        lightShieldDeath = false;
        lightShield.transform.parent = base.transform;
        lightShield.transform.localPosition = Vector3.zero;
        lightShield.transform.localScale = new Vector3(1f, 1f);
        lightShield.enabled = true;
        lightShield.color = Color.white;
        lightShieldActivated = true;
        lightShieldHP = 15;
    }

    public void Vanish()
    {
        if (!emanating && !hurt && !shattered)
        {
            frames = 0;
            emanating = true;
            emnDoReverse = true;
        }
    }

    public void ChangeSOULMode(int mode, bool makeSound = false)
    {
        ChangeSOULMode((SoulMode)mode, makeSound);
    }

    public void ChangeSOULMode(SoulMode mode, bool makeSound = false)
    {
        if ((bool)parryHitbox)
        {
            Object.Destroy(parryHitbox.gameObject);
        }
        SetGravityDirection(Vector2.down);
        SoulMode soulMode = this.soulMode;
        this.soulMode = mode;
        if ((bool)GetComponentInChildren<SOULChargeEffect>())
        {
            Object.Destroy(GetComponentInChildren<SOULChargeEffect>().gameObject);
        }
        if ((bool)drainGraphic)
        {
            Object.Destroy(drainGraphic.gameObject);
        }
        sr.sortingOrder = 199;
        grazer.GetComponent<CircleCollider2D>().radius = 0.45f;
        sr.flipY = this.soulMode == SoulMode.Shoot;
        grazer.GetComponent<SpriteRenderer>().flipY = sr.flipY;
        if (this.soulMode == SoulMode.Jump)
        {
            drainGraphic = Object.Instantiate(Resources.Load<GameObject>("battle/SOULDrainGraphic"), base.transform).GetComponent<SOULDrainGraphic>();
            drainGraphic.transform.localPosition = Vector3.zero;
        }
        else if (this.soulMode == SoulMode.Shield)
        {
            blueGravity = 0f;
            grazer.GetComponent<CircleCollider2D>().radius = 0.6f;
            sr.sortingOrder = 210;
        }
        else if (this.soulMode != SoulMode.Fall)
        {
            if (this.soulMode == SoulMode.Slow)
            {
                blueGravity = 0f;
                parryHitbox = new GameObject("ParryHitbox", typeof(SOULParryHitBox), typeof(SpriteRenderer), typeof(BoxCollider2D)).GetComponent<SOULParryHitBox>();
                parryHitbox.SetParentSOUL(this);
                parryHitbox.transform.parent = base.transform;
                parryHitbox.transform.localPosition = Vector3.zero;
                drainGraphic = Object.Instantiate(Resources.Load<GameObject>("battle/SOULDrainGraphic"), base.transform).GetComponent<SOULDrainGraphic>();
                drainGraphic.transform.localPosition = Vector3.zero;
            }
            else if (this.soulMode == SoulMode.Shoot)
            {
                blueGravity = 0f;
            }
            else if (this.soulMode == SoulMode.Trap)
            {
                blueGravity = 0f;
            }
            else
            {
                this.soulMode = SoulMode.Normal;
                blueGravity = 0f;
            }
        }
        AdjustSOULColor();
        if (soulMode != this.soulMode && makeSound)
        {
            FindFirstObjectByType<GameManager>().PlayGlobalSFX("sounds/snd_bell");
        }
    }

    public void AdjustSOULColor()
    {
        SoulMode soulMode = SoulMode.Normal;
        soulMode = ((this.soulMode != 0) ? this.soulMode : ((SoulMode)Util.GameManager().GetFlagInt(312)));
        soulColor = GetSOULColorByID((int)soulMode);
        sr.color = soulColor;
    }

    public void SetGravityDirection(Vector2 gravityDir)
    {
        this.gravityDir = gravityDir;
        if (gravityDir == Vector2.down)
        {
            base.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }
        if (gravityDir == Vector2.right)
        {
            base.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
        }
        if (gravityDir == Vector2.up)
        {
            base.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
        }
        if (gravityDir == Vector2.left)
        {
            base.transform.localRotation = Quaternion.Euler(0f, 0f, 270f);
        }
    }

    public void SlamToDirection(Vector2 gravityDir, float rate)
    {
        SetGravityDirection(gravityDir);
        int num = 516;
        num = ~num;
        RaycastHit2D raycastHit2D = Physics2D.Raycast(base.transform.position, gravityDir, float.PositiveInfinity, num);
        if (raycastHit2D.collider != null)
        {
            slamming = true;
            if (GravityIsVertical())
            {
                slamPosA = base.transform.position.y;
                slamPosB = raycastHit2D.point.y;
            }
            else
            {
                slamPosA = base.transform.position.x;
                slamPosB = raycastHit2D.point.x;
            }
            slamFrames = 0;
            slamMaxFrames = Mathf.RoundToInt(Mathf.Abs(slamPosA - slamPosB) * 48f / rate);
        }
    }

    public int GetSOULMode()
    {
        return (int)soulMode;
    }

    public Color GetSOULColor()
    {
        return soulColor;
    }

    public Color GetSpriteColor()
    {
        return sr.color;
    }

    public static Color GetSOULColorByID(int i, bool forceNormal = false)
    {
        return SOUL_COLORS[i];
    }

    public bool OnPlatform()
    {
        return curPlatform;
    }

    public void BulletTriggerEnter(Collider2D collision)
    {
        if (!collision.gameObject.tag.Contains("Bullet") || collision.gameObject.layer == 2 || !isPlayer)
        {
            return;
        }
        if ((collision.gameObject.tag.StartsWith("Blue") && isMoving) || (collision.gameObject.tag.StartsWith("Orange") && !isMoving) || collision.gameObject.tag == "Bullet")
        {
            /*
            MonoBehaviour.print("soul hit by " + collision.gameObject.name);
            collision.gameObject.GetComponentInParent<BulletBase>().PreSOULHit();
            DamageSOUL(collision);
            collision.gameObject.GetComponentInParent<BulletBase>().SOULHit();
            if ((bool)collision.gameObject.GetComponent<GrabPoint>())
            {
                inControl = false;
                isGrabbed = true;
                base.transform.SetParent(collision.transform, worldPositionStays: true);
                base.transform.localPosition = Vector3.zero;
            }
            */
        }
    }

    public void BulletTriggerStay(Collider2D collision)
    {
        if (collision.gameObject.tag.Contains("Bullet") && collision.gameObject.layer != 2 && isPlayer && ((collision.gameObject.tag.StartsWith("Blue") && isMoving) || (collision.gameObject.tag.StartsWith("Orange") && !isMoving) || collision.gameObject.tag == "Bullet"))
        {
            DamageSOUL(collision);
        }
    }

    public void DamageSOUL(Collider2D collision)
    {
        if (collision.gameObject.GetComponentInParent<BulletBase>().GetBaseDamage() > -1)
        {
            lastHitBullet = collision.GetComponent<BulletBase>();
            int num = collision.gameObject.GetComponentInParent<BulletBase>().GetBaseDamage();
            if (num < 1 && collision.gameObject.GetComponentInParent<BulletBase>().GetBaseDamage() > 0)
            {
                num = 1;
            }
            else if (collision.gameObject.GetComponentInParent<BulletBase>().GetBaseDamage() == 0)
            {
                num = 0;
            }
            Damage(num);
        }
    }


    public void DebugInv()
    {
        debugInv = !debugInv;
        hurt = true;
    }

    public void DebugMode()
    {
        int mode = (int)(soulMode + 1);
        ChangeSOULMode(mode, makeSound: true);
    }

    public void DebugDamage(int hp)
    {
        hurt = false;
        Damage(hp);
    }

    public bool IsPlayer()
    {
        return isPlayer;
    }

    public bool IsControllable()
    {
        return inControl;
    }

    public int GetInvFrames()
    {
        if (hurt)
        {
            return invFrames - frames;
        }
        return 0;
    }

    public void SetInvFrames(int invFrames)
    {
        this.invFrames = invFrames;
    }

    public void SetInvFrames(int invFrames, bool easyOverride)
    {
        this.invFrames = invFrames;
    }

    public void SetDamageMultiplier(float damageMulti)
    {
        this.damageMulti = damageMulti;
    }

    public int GetChargeFrames()
    {
        return bigShotCharge;
    }

    public int GetBigShotCheating()
    {
        return bigShotCheating;
    }

    public void EnableYDash()
    {
        yDashEnabled = true;
    }

    public void DisableYDash()
    {
        yDashEnabled = false;
    }

    public bool IsGrabbed()
    {
        return isGrabbed;
    }

    private bool IsJumping()
    {
        if ((!(UTInput.GetAxisRaw("Vertical") > 0f) || !(gravityDir == Vector2.down)) && (!(UTInput.GetAxisRaw("Vertical") < 0f) || !(gravityDir == Vector2.up)) && (!(UTInput.GetAxisRaw("Horizontal") > 0f) || !(gravityDir == Vector2.left)))
        {
            if (UTInput.GetAxisRaw("Horizontal") < 0f)
            {
                return gravityDir == Vector2.right;
            }
            return false;
        }
        return true;
    }

    private bool GravityIsVertical()
    {
        if (!(gravityDir == Vector2.down))
        {
            return gravityDir == Vector2.up;
        }
        return true;
    }

    private bool HitFloor(RaycastHit2D hit)
    {
        if (gravityDir == Vector2.down)
        {
            return hit.point.y - base.transform.position.y > -0.2f;
        }
        if (gravityDir == Vector2.up)
        {
            return hit.point.y - base.transform.position.y < 0.2f;
        }
        if (gravityDir == Vector2.left)
        {
            return hit.point.x - base.transform.position.x > -0.2f;
        }
        if (gravityDir == Vector2.right)
        {
            return hit.point.x - base.transform.position.x < 0.2f;
        }
        return false;
    }

    private bool HitCeil(RaycastHit2D hit)
    {
        if (-gravityDir == Vector2.down)
        {
            return hit.point.y - base.transform.position.y > -0.2f;
        }
        if (-gravityDir == Vector2.up)
        {
            return hit.point.y - base.transform.position.y < 0.2f;
        }
        if (-gravityDir == Vector2.left)
        {
            return hit.point.x - base.transform.position.x > -0.2f;
        }
        if (-gravityDir == Vector2.right)
        {
            return hit.point.x - base.transform.position.x < 0.2f;
        }
        return false;
    }

    public bool WasShotByGun()
    {
        return shot;
    }

    public Vector2 GetGravityDirection()
    {
        return gravityDir;
    }

    public bool IsMoving()
    {
        return isMoving;
    }

    public bool IsInvincible()
    {
        return hurt;
    }

    public SpriteRenderer GetShield()
    {
        return lightShield;
    }

    public bool IsShieldActive()
    {
        return lightShieldActivated;
    }


    public void SetCollision(bool enabled)
    {
        GetComponent<BoxCollider2D>().enabled = enabled;
    }
}
