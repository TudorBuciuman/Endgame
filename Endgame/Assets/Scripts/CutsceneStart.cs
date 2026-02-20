using UnityEngine;

public class CutsceneStart : OverworldManipulator
{
    [SerializeField]
    private int flag = -1;

    [SerializeField]
    private int cutsceneId;

    [SerializeField]
    private bool destroyOnTouch = true;

    private bool started;

    private CutsceneBase cutscene;

    protected override void Awake()
    {
        base.Awake();
        if (flag > -1)
        {
            if ((int)Object.FindObjectOfType<GameManager>().GetFlag(flag) >= 1)
            {
                Object.Destroy(base.gameObject);
                return;
            }
            while (base.transform.childCount > 0)
            {
                base.transform.GetChild(0).parent = null;
            }
        }
        else
        {
            while (base.transform.childCount > 0)
            {
                base.transform.GetChild(0).parent = null;
            }
        }
    }

    private void Start()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player" && !started)
        {
            started = true;
            cutscene = CutsceneHandler.GetCutscene(cutsceneId);
            cutscene.StartCutscene();
            if (destroyOnTouch)
            {
                Object.Destroy(base.gameObject);
            }
        }
    }

    public CutsceneBase GetCutscene()
    {
        return cutscene;
    }

    public int GetCutsceneID()
    {
        return cutsceneId;
    }
}
