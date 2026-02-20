using UnityEngine;
using UnityEngine.UI;

public class JumpDashTutorial : MonoBehaviour
{
    private Transform tutorialCanvas;

    private int frames;

    private bool triggered;

    private bool usingController;

    private void Update()
    {
        if (!triggered)
        {
            return;
        }
        frames++;
        if (frames == 60)
        {
            Text[] componentsInChildren = tutorialCanvas.GetComponentsInChildren<Text>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                componentsInChildren[i].enabled = true;
            }
            if (usingController)
            {
                tutorialCanvas.GetComponentInChildren<Image>().enabled = true;
            }
        }
        if (frames >= 60)
        {
            tutorialCanvas.Find("DirText").GetComponent<Text>().color = ((UTInput.GetAxis("Vertical") > 0f) ? new Color(1f, 1f, 0f) : ((Color)new Color32(160, 160, 160, byte.MaxValue)));
            tutorialCanvas.Find("ButtonText").GetComponent<Text>().color = (UTInput.GetButton("Z") ? new Color(1f, 1f, 0f) : ((Color)new Color32(160, 160, 160, byte.MaxValue)));
            if (usingController)
            {
                tutorialCanvas.GetComponentInChildren<Image>().color = (UTInput.GetButton("Z") ? Color.white : ((Color)new Color32(160, 160, 160, byte.MaxValue)));
            }
        }
    }

    private void OnDestroy()
    {
        if ((bool)tutorialCanvas)
        {
            Object.Destroy(tutorialCanvas.gameObject);
        }
    }

    public void Unfreeze()
    {
        //Object.FindObjectOfType<PlatformChallenge1>().SetFreeze(frozen: false);
        //Object.FindObjectOfType<Sans>().SetFreeze(frozen: false);
        //Object.FindObjectOfType<SansBG>().SetFreeze(frozen: false);
        Object.FindObjectOfType<BattleManager>().GetComponent<MusicPlayer>().Resume();
        GameObject.Find("FreezeFade").GetComponent<SpriteRenderer>().enabled = false;
        Object.FindObjectOfType<SOUL>().GetComponent<SpriteRenderer>().sortingOrder = 200;
        Object.FindObjectOfType<SOULGraze>().enabled = true;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.GetComponent<SOUL>() || triggered)
        {
            return;
        }
        triggered = true;
        GetComponent<AudioSource>().Play();
        //Object.FindObjectOfType<PlatformChallenge1>().SetFreeze(frozen: true);
        //Object.FindObjectOfType<Sans>().SetFreeze(frozen: true);
        //Object.FindObjectOfType<SansBG>().SetFreeze(frozen: true);
        Object.FindObjectOfType<BattleManager>().GetComponent<MusicPlayer>().Pause();
        GameObject.Find("FreezeFade").GetComponent<SpriteRenderer>().enabled = true;
        Object.FindObjectOfType<SOULGraze>().enabled = false;
        SOUL sOUL = Object.FindObjectOfType<SOUL>();
        sOUL.SetFrozen(boo: true);
        sOUL.GetComponent<SpriteRenderer>().sortingOrder = 500;
        if (sOUL.transform.position.y < -2.33f)
        {
            sOUL.transform.position = new Vector3(sOUL.transform.position.x, -2.33f);
        }
        tutorialCanvas = Object.Instantiate(Resources.Load<GameObject>("ui/JumpDashTutorialCanvas")).transform;
        Vector3 position = Object.FindObjectOfType<SOUL>().transform.position;
        position.x = (float)Mathf.RoundToInt(position.x * 48f) / 48f;
        position.y = (float)Mathf.RoundToInt(position.y * 48f) / 48f;
        tutorialCanvas.position = position;
        usingController = UTInput.joystickIsActive;
        tutorialCanvas.Find("ButtonText").GetComponent<Text>().text = tutorialCanvas.Find("ButtonText").GetComponent<Text>().text.Replace("[Z]", usingController ? "       " : string.Format("[{0}]", UTInput.GetButton("Confirm")));
        if (!usingController)
        {
            return;
        }
        //for (int i = 0; i < ButtonPrompts.validButtons.Length; i++)
        //{
            //if (UTInput.GetKeyOrButtonReplacement("Confirm") == ButtonPrompts.GetButtonChar(ButtonPrompts.validButtons[i]))
           // {
             //   tutorialCanvas.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("ui/buttons/" + ButtonPrompts.GetButtonGraphic(ButtonPrompts.validButtons[i]));
            //    break;
            //}
        //}
    }
}
