using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public void Start()
    {
        StartCoroutine(Presser());
        GameManager.instance.SetMenuDis();
    }
    void Update()
    {
        if (pressedZ)
        {
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
        audioSource.clip = song;
        while (true)
        {
            if(Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
            {
                yield return new WaitForSeconds(0.2f);
                logo.SetActive(false);
                audioSource.Play();
                pressedZ = true;
                yield break;
            }
            yield return null;
        }
    }
}
