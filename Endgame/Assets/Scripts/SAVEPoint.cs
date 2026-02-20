using UnityEngine;

public class SAVEPoint : Interactable
{
    [SerializeField]
    private bool doPhrase;

    [SerializeField]
    private string[] phrases = new string[1] { "* (You're filled with\n  determination.)" };

    [SerializeField]
    private string relativeSpawn = "down";

    private GameManager gm;

    private bool isSaving;

    private bool saveMenuOpen;

    [SerializeField]
    private bool force;

    [SerializeField]
    private bool allowOblitModification = true;

    private void Awake()
    {
        isSaving = false;
    }

    private void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        if ((int)gm.GetFlag(13) <= 1 || !allowOblitModification)
        {
            return;
        }
            phrases = new string[1] { "* You are filled with power." };
    }

    private void Update()
    {
        if (isSaving && txt == null && !saveMenuOpen)
        {
            Object.Instantiate(Resources.Load<GameObject>("ui/SaveMenu"), Vector3.zero, Quaternion.identity, GameObject.Find("Canvas").transform).transform.localPosition = Vector3.zero;
            saveMenuOpen = true;
        }
        if (saveMenuOpen && !FindFirstObjectByType<SaveMenu>())
        {
            saveMenuOpen = false;
            isSaving = false;
            gm.canMove = true;
        }
    }

    public override void DoInteract()
    {
        gm.Heal(99);
        //gm.PlayGlobalSFX("sounds/snd_heal");
        gm.canMove = false;
        gm.SetMenuDisabled(true);
        if (doPhrase)
        {
            txt = new GameObject().AddComponent<TextBox>();
            txt.CreateBox(phrases, "snd_text", 1,giveBackControl: false);
        }
        isSaving = true;
    }

    public override void MakeDecision(Vector2 index, int id)
    {
        isSaving = false;
    }

    public Vector3 GetSpawnPosition()
    {
        float num = 1f;
        if (relativeSpawn == "up" || relativeSpawn == "down")
        {
            if (relativeSpawn == "down")
            {
                num = -1f;
            }
            return base.transform.position + new Vector3(0f, 1.2f * num);
        }
        if (relativeSpawn == "left" || relativeSpawn == "right")
        {
            if (relativeSpawn == "left")
            {
                num = -1f;
            }
            return base.transform.position + new Vector3(num, 0.4f);
        }
        return base.transform.position + new Vector3(0f, -1.2f);
    }

    public void ModifyPhrases(string[] lines)
    {
        phrases = lines;
    }

    public void CancelSave()
    {
        isSaving = false;
        saveMenuOpen = false;
    }

    public bool IsSaving()
    {
        return isSaving;
    }

    public override int GetEventData()
    {
        return -1;
    }
}
