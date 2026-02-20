using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    // Start is called before the first frame update
    private bool pressedZ=false;
    public AudioClip song;
    public AudioSource audioSource;
    public GameObject logo;
    private int index=1;
    public Text Resume;
    public Text Reset;
    public Text Sett;
    public Color yellow =new(255, 255, 0);
    public Text LV, TIME, ZONE;
    public static bool canSkip = false;
    public void Start()
    {
        GameManager.instance.SetMenu(true);
        if (canSkip)
        {
            canSkip = false;
            logo.SetActive(false);
            pressedZ = true;
        }
        else
        {
            FindFirstObjectByType<GameManager>().PlayMusic(song);
            audioSource.Play();
            StartCoroutine(Presser());
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        LV.text = "LV "+GameManager.instance.GetLV().ToString();
        TIME.text = GameManager.instance.GetFormattedUpdatedPlayTime();
        ZONE.text = MapInfo.GetMapName(GameManager.instance.GetZone());
        //GameManager.instance.SetMenuDis();
    }
    void Update()
    {
        if (pressedZ)
        {
            if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (index == 1)
                {
                    GoToScene(GameManager.instance.GetZone());
                    GameManager.instance.SetMenuToBeOpened();
                }
                else if (index == 3)
                    SceneManager.LoadScene("Settings");
                else
                {
                    //
                }
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (index == 1 || index == 2)
                {
                    ChangeColor(Color.white, 1);
                    ChangeColor(Color.white, 2);
                    ChangeColor(yellow, 3);
                    index = 3;
                }
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (index == 3)
                {
                    ChangeColor(Color.white, 3);
                    ChangeColor(yellow, 1);
                    index = 1;
                }
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (index == 1)
                {
                    ChangeColor(Color.white, 1);
                    ChangeColor(yellow, 2);
                    index = 2;
                }
                else if (index == 2)
                {
                    ChangeColor(Color.white, 2);
                    ChangeColor(yellow, 1);
                    index = 1;
                }
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (index == 2)
                {
                    ChangeColor(Color.white, 2);
                    ChangeColor(yellow, 1);
                    index = 1;
                }
                else if (index == 1)
                {
                    ChangeColor(Color.white, 1);
                    ChangeColor(yellow, 2);
                    index = 2;
                }
            }
        }
    }
    private void ChangeColor(Color col, int y)
    {
        switch (y)
        {
            case 1:
                Resume.color = col;
                break;
            case 2:
                Reset.color = col;
                break;
            case 3:
                Sett.color = col;
                break;
        }
    }
    public IEnumerator Presser()
    {
        
        while (true)
        {
            if(Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
            {
                yield return new WaitForSeconds(0.2f);
                logo.SetActive(false);
                pressedZ = true;
                yield break;
            }
            yield return null;
        }
    }
    public void GoToScene(int scene)
    {
        GameManager.instance.StartTrackTime();
        GameManager.instance.LoadArea(scene, true, GameManager.instance.save.pos, 2);
    }
}
