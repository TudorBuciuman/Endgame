using UnityEngine;

public class LoadingZone : OverworldManipulator
{
    [SerializeField]
    private int newScene = 2;

    [SerializeField]
    private Vector2 newPos = Vector2.zero;

    [SerializeField]
    private byte face = 0;

    [SerializeField]
    private int fadeType;

    [SerializeField]
    private int forceActivationFlag = -1;

    [SerializeField]
    private string denyText = "* (You felt that you shouldn't\n  advance.)";

    [SerializeField]
    private string denySound = "snd_text";

    [SerializeField]
    private string denyPortrait = "";

    [SerializeField]
    private Vector3 denyNudge = Vector3.zero;

    [SerializeField]
    private bool fadeMusic;

    private bool forceActivationTrigger;

    private Fade fade;

    private bool activated;

    private bool punchCardDetected;

    private void Start()
    {
        fade = FindFirstObjectByType<Fade>();
        activated = false;
    }

    private void Update()
    {
        if (!activated)
        {
            return;
        }
        else
        {
            FindFirstObjectByType<GameManager>().LoadArea(newScene, fadeIn: true, newPos, face);
        }
        activated = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        /*
        if (!(collision.transform.tag == "Player") || activated || FindFirstObjectByType<OverworldPlayer>().IsInitiatingBattle())
        {
            return;
        }
        /*
        if ((forceActivationFlag > -1 && (int)FindFirstObjectByType<GameManager>().GetFlag(forceActivationFlag) == 0) || forceActivationTrigger)
        {
            FindFirstObjectByType<GameManager>().DisablePlayerMovement(deactivatePartyMembers: false);
            //FindFirstObjectByType<OverworldPlayer>().transform.position += denyNudge;
            new GameObject("txt").AddComponent<TextBox>().CreateBox(new string[1] { denyText }, new string[1] { denySound }, new int[1], new string[1] { denyPortrait });
            return;
        }
        */
        //GameObject.Find("GameManager").GetComponent<GameManager>().DisablePlayerMovement(deactivatePartyMembers: true);
        activated = true;
    }

    public void SetForceActivationTrigger(bool forceActivationTrigger)
    {
        this.forceActivationTrigger = forceActivationTrigger;
    }

    public void ModifyContents(string text, string sound, string portrait)
    {
        denyText = text;
        denySound = sound;
        denyPortrait = portrait;
    }

    public int GetScene()
    {
        return newScene;
    }
}
