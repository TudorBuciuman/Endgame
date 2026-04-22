using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Fighting : MonoBehaviour
{
    public AudioClip yeezy_lacrimosa;
    public AudioClip violence;
    public AudioClip moonlight;
    public AudioClip IamNotHome_OnSight;
    public AudioSource audioSource;
    public AudioSource soundsSource;
    public AudioClip select;

    public GameObject mountain_dark;
    public GameObject mountain_bright1,mountain_bright2;
    public GameObject mountain_halo;
    public GameObject light_beamed;

    public GameObject Fight;
    public Text OminousNarator;
    

    public GameObject[] pieces=new GameObject[6];
    public SpriteRenderer brightener;
    public GameObject NamesObj;
    public Text[] names=new Text[6];
    public int index = 0;
    public Color yellow = new(255, 255, 0);
    int currentLine = 0;

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        StartCoroutine(LightBeamed());
    }

    public IEnumerator LightBeamed()
    {
        //StartCoroutine(UpdateBrightness());
        audioSource.clip = yeezy_lacrimosa;
        yield return new WaitForSeconds(3);
        mountain_dark.SetActive(true);
        yield return new WaitForSeconds(5);
        audioSource.Play();
        yield return new WaitForSeconds(1);
        TriggerShake();
        yield return new WaitForSeconds(89);

        yield return StartCoroutine(Fightinging());
        StartCoroutine(Choice());
    }
    public IEnumerator IntoTheWorld()
    {
        yield return null;
    }
    public IEnumerator LightFelt()
    {
        yield return new WaitForSeconds(2);
        light_beamed.gameObject.SetActive(true);
    }

    public IEnumerator Fightinging()
    {
        light_beamed.SetActive(false);
        Fight.SetActive(true);
        audioSource.clip = violence;
        audioSource.Play();
        yield return new WaitForSeconds(40);
        Fight.SetActive(false);
        yield return null;
    }
    public IEnumerator Choice()
    {
        //soundplay
        dialogueLines = idk;
        for (int i = 0; i < dialogueLines.Length; i++)
        {
            dialogueLines[i] = dialogueLines[i].Replace("\\n ", "\n");
        }
        OminousNarator.gameObject.SetActive(true);
        StartCoroutine(DisplayNextLine());
        yield return new WaitForSeconds(60);
        OminousNarator.text = null;
        audioSource.clip = moonlight;
        audioSource.Play();
        yield return new WaitForSeconds(3);
        brightener.gameObject.SetActive(true);
        StartCoroutine(FadeOut(5));
        pieces[0].SetActive(true);
        NamesObj.SetActive(true);
        yield return new WaitForSeconds(5);

        StartCoroutine(InputGetter());
    }
    public IEnumerator FadeOut(float t)
    {
        Color c = brightener.color;
        float startAlpha = c.a;
        float elapsed = 0f;

        while (elapsed < t)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(startAlpha, 0f, elapsed / t);
            brightener.color = c;
            yield return null;
        }

        c.a = 0f;
        brightener.color = c;
    }
    public IEnumerator InputGetter()
    {
        float t = 0;
        while (t<90)
        {
            t += Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.LeftArrow) || UTInput.GetAxisRaw("Horizontal")<0)
            {
                if (index != 0)
                {
                    names[index].color = Color.white;
                    pieces[index].SetActive(false);
                    index--;
                    names[index].color = yellow;
                    pieces[index].SetActive(true);
                }
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) || UTInput.GetAxisRaw("Horizontal") > 0)
            {
                if (index != 5)
                {
                    names[index].color = Color.white;
                    pieces[index].SetActive(false);
                    index++;
                    names[index].color = yellow;
                    pieces[index].SetActive(true);

                }
            }
            yield return null;
        }
        pieces[index].SetActive(false);
        yield return StartCoroutine(Falling());
        t=0;
        while (t < 90)
        {
            t += Time.deltaTime;
            if(Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || UTInput.GetButtonDown("Z"))
            {
                yield return new WaitForSeconds(0.3f);
                t = 91;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || UTInput.GetAxisRaw("Horizontal") < 0)
            {
                if (index != 0)
                {
                    names[index].color = Color.white;
                    index--;
                    names[index].color = yellow;
                }
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) || UTInput.GetAxisRaw("Horizontal") > 0)
            {
                if (index != 5)
                {
                    names[index].color = Color.white;
                    index++;
                    names[index].color = yellow;
                }
            }
            yield return null;
        }
        soundsSource.clip = select;
        soundsSource.Play();
        NamesObj.SetActive(false);
        yield return new WaitForSeconds(3);
        dialogueLines = idk2;
        for (int i = 0; i < dialogueLines.Length; i++)
        {
            dialogueLines[i] = dialogueLines[i].Replace("\\n ", "\n");
        }
        OminousNarator.gameObject.SetActive(true);
        currentLine = 0;
        StartCoroutine(DisplayNextLine());
        yield return new WaitForSeconds(20);
        OminousNarator.text = null;
        yield return new WaitForSeconds(2);
        StartCoroutine(OnSight());
    }
    public IEnumerator Falling()
    {
        Vector2 c = NamesObj.transform.position;
        Vector2 f = new(0, -4.7f);
        float e = 0;

        while (e < 4)
        {
            e += Time.deltaTime;
            NamesObj.transform.position = new Vector2(0,Mathf.Lerp(c.y,f.y, e / 4));
            yield return null;
        }

        NamesObj.transform.position = f;
        yield return null;
    }
    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPosition = transform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = elapsed * Random.Range(-1f, 1f) * magnitude;
            float y = elapsed * Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPosition;
    }
    public void TriggerShake()
    {
        light_beamed.SetActive(true);
        mountain_dark.SetActive(false);
        StartCoroutine(Shake(20f, 0.003f));
    }
    public IEnumerator DisplayNextLine()
    {
        if (currentLine < dialogueLines.Length)
        {
            yield return StartCoroutine(TypeLine(dialogueLines[currentLine].Trim(), 0.09f));
        }
        else
            yield return null;

    }
    private IEnumerator TypeLine(string line, float spd)
    {
        OminousNarator.text = "";
        foreach (char c in line)
        {
            OminousNarator.text += c;
            yield return new WaitForSeconds(spd);
        }
        currentLine++;
        yield return new WaitForSeconds(1.4f);
        StartCoroutine(DisplayNextLine());
    }
    public IEnumerator OnSight()
    {
        mountain_dark.SetActive(true);
        yield return StartCoroutine(SlowDown(4));
        yield return new WaitForSeconds(2);
        audioSource.clip = IamNotHome_OnSight;
        audioSource.volume = 100;
        audioSource.Play();
        mountain_dark.SetActive(true);
        //shake low
        StartCoroutine(Shake(150f, 0.001f));
        yield return new WaitForSeconds(153);
        mountain_halo.SetActive(true);
        mountain_dark.SetActive(false);
        PlayerPrefs.SetInt("sawIntro", 10);
        yield return new WaitForSeconds(55.3f);
        mountain_halo.SetActive(false);
        mountain_dark.SetActive(true);
        yield return StartCoroutine(Shake(60f, 0.005f));
        yield return new WaitForSeconds(2);
        mountain_dark.SetActive(false);
        yield return new WaitForSeconds(7);
        SceneManager.LoadScene("Intro");

    }
    public IEnumerator SlowDown(float t)
    {
        float tm = 0;
        while (tm < t)
        {
            tm += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(100, 0, t / tm);
            yield return null;
        }
        audioSource.Stop();
        yield return null;
    }
    public string[] dialogueLines;
    private string[] idk =
    {
        "Light bled into the world...",
        "...and with it came the \nfirst game.",
        "Kings rose. Armies stood. \nThe war never ended.",
        "You.. stood at the edge.",
        "One step forward.",
        "The world called it \ndestiny.",
        "You called it...  \nyour turn.",
        "Some rise to lead.",
        "Some stand as walls.",
        "Some cut the field \nin silence.",
        "Some bend the rules \nthemselves.",
        "And some...",
        "... are sent to \nfall first.",
        " ",
        "Choose your place."

    };
    private string[] idk2 =
    {
        "You think you chose.",
        "You thought you were \nmeant for more.",
        "But no one will shed \na tear for you.",
        "Pawn."
    };
    public GameObject targetImage;
    public float brightnessMultiplier = 2f;
    public float smoothSpeed = 5f;
    public float updateInterval = 0.02f;
    private float[] spectrumData;
    private Color baseColor;
    private int sampleSize = 64; 
    public FFTWindow fftWindow = FFTWindow.Blackman; 
    public int frequencyIndex = 5;
    public IEnumerator UpdateBrightness()
    {
        spectrumData = new float[sampleSize];
        baseColor = targetImage.GetComponent<SpriteRenderer>().color;

        //StartCoroutine(UpdateBrightness());

        while (true)
        {
            audioSource.GetSpectrumData(spectrumData, 0, fftWindow);

            float value = Mathf.Clamp01(spectrumData[frequencyIndex] * brightnessMultiplier);

            float targetBrightness = Mathf.Lerp(1.2f, 2f, value); // 1 = normal, 2 = double brightness
            Color newColor = baseColor * targetBrightness;
            newColor.a = baseColor.a; 
            targetImage.GetComponent<SpriteRenderer>().color = Color.Lerp(targetImage.GetComponent<SpriteRenderer>().color, newColor, Time.deltaTime * smoothSpeed);

            yield return new WaitForSeconds(updateInterval);
        }
    }
}
