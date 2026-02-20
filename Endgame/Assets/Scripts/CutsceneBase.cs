using System;
using UnityEngine;

public class CutsceneBase : TranslatableSelectableBehaviour
{
    protected GameManager gm;

    protected int frames;

    protected int state;

    protected bool isPlaying;

    protected TextBox txt;

    protected Selection sels;

    protected DeltaSelection select;

    protected AudioSource aud;

    protected CameraController cam;

    protected Fade fade;

    protected PlayerController pawn;

    private int diagCheck = -1;

    protected void Awake()
    {
        stringSubFolder = "cutscene";
        frames = 0;
        state = 0;
        cam = FindFirstObjectByType<CameraController>();
        aud = base.gameObject.AddComponent<AudioSource>();
        pawn = FindFirstObjectByType<PlayerController>();
        gm = FindFirstObjectByType<GameManager>();
        fade = FindFirstObjectByType<Fade>();
    }

    public virtual void StartCutscene(params object[] par)
    {
        int num = 0;
        SetStrings(GetDefaultStrings(), GetType());
        if ((bool)FindFirstObjectByType<PlayerController>())
        {
            FindFirstObjectByType<PlayerController>().SetCollision(onoff: false);
        }
        try
        {
            num = int.Parse(par[0].ToString());
        }
        catch (IndexOutOfRangeException)
        {
            Debug.LogWarning("CutsceneBase: Intended skip value doesn't exist.");
        }
        catch (FormatException)
        {
            Debug.LogWarning("CutsceneBase: Intended skip value not an int. Ignoring.");
        }
        if (num == -1)
        {
            EndCutscene();
            return;
        }
        gm.DisablePlayerMovement(deactivatePartyMembers: true);
        isPlaying = true;
    }

    public bool IsPlaying()
    {
        return isPlaying;
    }

    public virtual void EndCutscene(bool enablePlayerMovement = true)
    {
        isPlaying = false;
        if (enablePlayerMovement)
        {
            if ((bool)FindFirstObjectByType<PlayerController>())
            {
                FindFirstObjectByType<PlayerController>().SetCollision(onoff: true);
            }
            gm.EnablePlayerMovement();
        }
        UnityEngine.Object.Destroy(base.gameObject);
    }

    protected void StartText(string[] text, string[] sounds, int[] speeds, string[] portraits, int pos = -1)
    {
        diagCheck = -1;
        txt = new GameObject("CutsceneText").AddComponent<TextBox>();
        if (pos == -1)
        {
            txt.CreateBox(text, sounds, speeds, giveBackControl: false, portraits);
        }
        else
        {
            txt.CreateBox(text, sounds, speeds, pos, giveBackControl: false, portraits);
        }
    }

    protected void StartText(TextSet textSet)
    {
        diagCheck = -1;
        txt = new GameObject("CutsceneText").AddComponent<TextBox>();
        if (textSet.location == -1)
        {
            textSet.location = 0;
        }
        txt.CreateBox(textSet);
    }

    protected void StartSelection(string optionA, string optionB, int defaultNextState, int pos)
    {
        if (txt != null && txt.AtLastText() && !txt.IsPlaying() && sels == null)
        {
            txt.EnableChoice();
            int num = 0;
            if ((GameObject.Find("Player").transform.position[1] - GameObject.Find("Camera").transform.position[1] < -0.9f && pos == -1) || pos == 0)
            {
                num = 310;
            }
            sels = txt.GetUIBox().AddComponent<Selection>();
            sels.CreateSelections(new string[1, 2] { { optionA, optionB } }, new Vector2(-116f, -283 + num), new Vector2(192f, 0f), new Vector2(-15f, 94f), "DTM-Mono", useSoul: true, makeSound: false, this, defaultNextState);
        }
    }

    protected void InitiateDeltaSelection()
    {
        select = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("ui/DeltaSelection"), Vector3.zero, Quaternion.identity, txt.GetUIBox().transform).GetComponent<DeltaSelection>();
    }

    public override void MakeDecision(Vector2 index, int id)
    {
        UnityEngine.Object.Destroy(txt.gameObject);
        state = id + (int)index[1];
    }

    protected void PlaySFX(string clip, float pitch = 1f)
    {
        aud.clip = Resources.Load<AudioClip>(clip);
        aud.pitch = pitch;
        aud.Play();
    }

    protected bool AtLine(int line)
    {
        if ((bool)txt && line == txt.GetCurrentStringNum() && txt.GetCurrentStringNum() > diagCheck)
        {
            diagCheck = txt.GetCurrentStringNum();
            return true;
        }
        return false;
    }

    protected bool AtLineRepeat(int line)
    {
        if ((bool)txt && line == txt.GetCurrentStringNum())
        {
            return true;
        }
        return false;
    }

    protected bool MoveTo(Component obj, Vector3 pos, float speed)
    {
        obj.transform.position = Vector3.MoveTowards(obj.transform.position, pos, speed / 48f);
        if (Vector3.Distance(obj.transform.position, pos) < 1f / 48f)
        {
            obj.transform.position = pos;
        }
        return obj.transform.position != pos;
    }

    protected bool LocalMoveTo(Component obj, Vector3 pos, float speed)
    {
        obj.transform.localPosition = Vector3.MoveTowards(obj.transform.localPosition, pos, speed / 48f);
        if (Vector3.Distance(obj.transform.localPosition, pos) < 1f / 48f)
        {
            obj.transform.localPosition = pos;
        }
        return obj.transform.localPosition != pos;
    }

    protected void SetSprite(Component obj, string spriteName, bool flipX = false)
    {
        StopAnimating(obj);
        if (obj.GetType() == typeof(PlayerController))
        {
            ((PlayerController)obj).SetSprite(spriteName);
        }
        else
        {
            Sprite sprite = Resources.Load<Sprite>(spriteName);
            if (sprite != null)
            {
                obj.GetComponent<SpriteRenderer>().sprite = sprite;
            }
        }
        obj.GetComponent<SpriteRenderer>().flipX = flipX;
    }

    protected void StopAnimating(Component obj)
    {
        if ((bool)obj.GetComponent<Animator>())
        {
            obj.GetComponent<Animator>().enabled = false;
        }
    }

    protected void StartAnimating(Component obj)
    {
        if ((bool)obj.GetComponent<Animator>())
        {
            obj.GetComponent<Animator>().enabled = true;
        }
    }

    protected void PlayAnimation(Component obj, string animName, float speed = 1f, bool startAtBeginning = true)
    {
        if ((bool)obj.GetComponent<Animator>())
        {
            obj.GetComponent<Animator>().enabled = true;
            if (startAtBeginning)
            {
                obj.GetComponent<Animator>().Play(animName, 0, 0f);
            }
            else
            {
                obj.GetComponent<Animator>().Play(animName);
            }
            obj.GetComponent<Animator>().SetFloat("speed", speed);
        }
    }

    protected void SetMoveAnim(Component obj, bool isMoving, float speed = 1f)
    {
        if ((bool)obj.GetComponent<Animator>())
        {
            obj.GetComponent<Animator>().SetBool("isMoving", isMoving);
            obj.GetComponent<Animator>().SetFloat("speed", speed);
        }
    }

    protected void ChangeDirection(Component obj, Vector2 direction)
    {
        if ((bool)obj.GetComponent<Animator>())
        {
            obj.GetComponent<Animator>().SetFloat("dirX", direction.x);
            obj.GetComponent<Animator>().SetFloat("dirY", direction.y);
        }
    }

    protected void LookAt(Component obj, Component lookat)
    {
        LookAt(obj, lookat, Vector3.zero);
    }

    protected void LookAt(Component obj, Component lookat, Vector3 offset)
    {
        ChangeDirection(obj, lookat.transform.position - obj.transform.position + offset);
    }

    protected void LookAway(Component obj, Component lookat)
    {
        LookAway(obj, lookat, Vector3.zero);
    }

    protected void LookAway(Component obj, Component lookat, Vector3 offset)
    {
        ChangeDirection(obj, obj.transform.position - lookat.transform.position + offset);
    }

    protected void RestorePlayerControl()
    {
        pawn.SetSelfAnimControl(setAnimControl: true);
        cam.SetFollowPlayer(follow: true);
    }

    protected void RevokePlayerControl()
    {
        gm.DisablePlayerMovement(deactivatePartyMembers: true);
        pawn.SetSelfAnimControl(setAnimControl: false);
        cam.SetFollowPlayer(follow: false);
    }
}
