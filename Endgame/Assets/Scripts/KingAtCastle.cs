using UnityEngine;

public class KingAtCastle : CutsceneBase
{
    private Animator pawn;
    private Animator king;

    private float tme = 0;
    private float runspd = 2;

    private void FixedUpdate()
    {
        if (!isPlaying)
        {
            return;
        }
        if (state == 0 && !txt)
        {

            if (frames == 1)
            {
                FindFirstObjectByType<PlayerController>().Lock();
                pawn.Play("walk");
                pawn.SetBool("isMoving", value: true);
                frames = 2;
            }
            if (FindFirstObjectByType<Rigidbody2D>().transform.position.y<-8f)
            {
                //tme += Time.deltaTime;
                FindFirstObjectByType<Rigidbody2D>().MovePosition(FindFirstObjectByType<Rigidbody2D>().transform.position + Vector3.up * runspd * Time.deltaTime);
            }
            else if (frames == 2)
            {
                frames = 3;
            }
            else if (frames == 3)
            {
                pawn.SetBool("isMoving", value: false);
                FindFirstObjectByType<CameraController>().SetFollowPlayer(false);
                frames = 4;
            }
            else if (frames == 4 && FindFirstObjectByType<CameraController>().gameObject.transform.localPosition.y<-4.5f)
            {
                FindFirstObjectByType<CameraController>().gameObject.transform.localPosition+=(Vector3.up * runspd * Time.deltaTime);

            }
            else
            {
                frames++;
            }
            
            if (frames == 120)
            {
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = 30;
                StartText(new string[2] { "* The world is full \n\b  of beauty.", "* Yet...^20 \n\b  we are still at war." }, new string[2] { "snd_txtasg", "snd_txtasg" }, new int[2] { 1, 1 }, new string[2] { "asg", "asg" });
            }
            if (frames == 160)
            {
                StartText(new string[2] { "* ...", "* Friends...^15 \n\b  Families..." }, new string[2] { "snd_txtasg", "snd_txtasg" }, new int[2] { 3, 1 }, new string[2] { "asg_sad", "asg_sad" });
            }
            if (frames == 200)
            {
                StartText(new string[2] { "* Though valiantly they\n\b  fought...", "* Nobody could stop this,^5 \n\b  could they?" }, new string[2] { "snd_txtasg", "snd_txtasg" }, new int[2] { 1, 1 }, new string[2] { "asg", "asg" });
            }
            if (frames == 230)
            {
                StartText(new string[3] { "* The world can be a cruel\n\b  place.", "* It gives and takes as it\n\b  pleases.", "* No one truly wins,^5 \n\b  not even kings." }, new string[2] { "snd_txtasg", "snd_txtasg" }, new int[2] { 1, 1 }, new string[2] { "", "" });
            }
            if (frames == 250)
            {
                StartText(new string[2] { "* I've tried to end this \n\b  war more times than \n\b  I can count.", "* But it is a conflict\n\b  not of our making." }, new string[2] { "snd_txtasg", "snd_txtasg" }, new int[2] { 1, 1 }, new string[2] { "", "" });
            }
            if (frames == 290)
            {
                StartText(new string[3] { "* ... ", "* One thing is certain,^5 \n\b  however.", "* The violence has\n\b  ensured this war never\n\b  to end." }, new string[3] { "snd_txtasg", "snd_txtasg", "snd_txtasg" }, new int[3] { 1, 1 ,1}, new string[3] { "", "","" });
            }
            if (frames == 310)
            {
                StartText(new string[1] { "* Many more, of both our\n\b  kinds, will perish\n\b  again and again." }, new string[1] { "snd_txtasg"}, new int[1] { 1}, new string[1] { "asg_sad"});
            }
            if (frames == 330)
            {
                StartText(new string[1] { "* I suppose I have talked\n\b  long enough." }, new string[1] { "snd_txtasg" }, new int[1] { 1 }, new string[1] { "" });
            }
            if (frames == 370)
            {
                StartText(new string[3] { "* We should go.", "* Otherwise...", "* We'll be late\n\b  for the game." }, new string[3] { "snd_txtasg", "snd_txtasg", "snd_txtasg" }, new int[3] { 1, 1 ,1}, new string[3] { "asg", "asg","asg" });
            }
            if (frames == 399)
            {
                QualitySettings.vSyncCount = 1;
                Application.targetFrameRate = 60;
                GameObject.Find("King").GetComponent<Animator>().enabled = true;
                GameObject.Find("King").GetComponent<Animator>().speed = 1f;
            }
            if (frames == 400 && GameObject.Find("King").gameObject.transform.localPosition.y<8)
            {
                GameObject.Find("King").gameObject.transform.localPosition += (Vector3.up * runspd * Time.deltaTime);
                frames--;
            }
            else if(GameObject.Find("King").gameObject.transform.localPosition.y >= 8)
            {
                frames = 500;
                pawn.Play("walk");
                pawn.SetBool("isMoving", value: true);
            }
            if (frames == 500 && FindFirstObjectByType<Rigidbody2D>().transform.position.y < 8f)
            {
                //tme += Time.deltaTime;
                FindFirstObjectByType<Rigidbody2D>().MovePosition(FindFirstObjectByType<Rigidbody2D>().transform.position + Vector3.up * runspd * Time.deltaTime);
                frames--;
            }
            else if(FindFirstObjectByType<Rigidbody2D>().transform.position.y >= 8f)
            {
                gm.InstantFade(60);
                TextBox.CanSkip = true;

            }
        }
    }

    public override void StartCutscene(params object[] par)
    {
        TextBox.CanSkip = false;
        base.StartCutscene(par);
        gm.DisablePlayerMovement(deactivatePartyMembers: true);
        gm.StopMusic(180);
        gm.SetFlag(5, 1);
        pawn = GameObject.Find("Player").GetComponent<Animator>();
        king = GameObject.Find("King").GetComponent<Animator>();
        frames = 1;
    }
}
