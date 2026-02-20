using System.Collections;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private AudioSource src;

    private AudioSource src2;

    private AudioClip audioIntro;

    private AudioClip audioLoop;

    [SerializeField]
    private string musicName = "";

    [SerializeField]
    private bool hasIntro;

    [SerializeField]
    private bool isPlaying;

    [SerializeField]
    private bool isOverlap;

    private int isOverlapFrames;

    [SerializeField]
    private int isOverlapMaxFrames;

    private bool fadingOut;

    private bool fadingIn;

    private float frames = 30f;

    private float baseVolume = 1f;

    private float pauseSecs;

    private bool isPaused;

    private void Awake()
    {
        if (src == null)
        {
            src = base.gameObject.AddComponent<AudioSource>();
        }
        audioLoop = Resources.Load<AudioClip>(musicName);
        src.clip = audioLoop;
        if (hasIntro)
        {
            src.loop = false;
            audioIntro = Resources.Load<AudioClip>(musicName + "_intro");
            src.clip = audioIntro;
            Play();
        }
        if (isOverlap)
        {
            src2 = base.gameObject.AddComponent<AudioSource>();
            src2.clip = audioLoop;
        }
        else if (isPlaying && !hasIntro)
        {
            src.loop = true;
            src.Play();
        }
    }

    private void Update()
    {
        if (isOverlap && isPlaying)
        {
            if (isOverlapFrames == 0 && !src.isPlaying)
            {
                src.Play();
            }
            if (isOverlapFrames >= isOverlapMaxFrames)
            {
                if (src.isPlaying)
                {
                    src2.Play();
                }
                else if (src2.isPlaying)
                {
                    src.Play();
                }
                isOverlapFrames = 0;
            }
            else
            {
                isOverlapFrames++;
            }
        }
        else if (fadingOut)
        {
            if (src.volume <= 0f)
            {
                fadingOut = false;
                Stop();
            }
            src.volume -= frames;
        }
        else if (fadingIn)
        {
            if (src.volume >= 1f)
            {
                fadingIn = false;
            }
            src.volume += frames;
        }
        else if (isPlaying && !fadingOut)
        {
            if (musicName.Contains("deeploop2"))
            {
                src.volume = 0.5f;
            }
            else
            {
                src.volume = baseVolume;
            }
        }
    }

    public void Play()
    {
        if (!isOverlap)
        {
            StartCoroutine(PlayMusic());
        }
        isPlaying = true;
    }

    public bool IsPlaying()
    {
        return isPlaying;
    }

    public bool IsPaused()
    {
        if (!isPlaying)
        {
            return isPaused;
        }
        return false;
    }

    public void Stop()
    {
        StopAllCoroutines();
        if (!isOverlap)
        {
            src.loop = false;
        }
        else
        {
            src2.Stop();
        }
        src.Stop();
        if ((bool)GetComponentInChildren<LostCoreMusic>())
        {
            Object.Destroy(GetComponentInChildren<LostCoreMusic>().gameObject);
        }
        isPlaying = false;
    }

    public void FadeOut(float sec)
    {
        if ((bool)GetComponentInChildren<LostCoreMusic>())
        {
            GetComponentInChildren<LostCoreMusic>().FadeOut((int)(sec * 30f));
            return;
        }
        frames = 1f / sec / 30f;
        fadingOut = true;
    }

    public void ChangeMusic(string name, bool intro, bool playImmediately, bool overlap, int overlapMaxFrames)
    {
        StopAllCoroutines();
        pauseSecs = 0f;
        src.time = 0f;
        if (fadingOut && playImmediately)
        {
            fadingOut = false;
            Stop();
            src.volume = baseVolume;
        }
        if (IsPlaying())
        {
            Stop();
        }
        isPaused = false;
        musicName = name;
        if (name.EndsWith("mus_lostcore"))
        {
            Object.Instantiate(Resources.Load<GameObject>("overworld/lostcore_objects/LostCoreMusic"), base.transform);
            isPlaying = true;
            return;
        }

        hasIntro = intro;
        isPlaying = playImmediately;
        isOverlap = overlap;
        isOverlapFrames = 0;
        isOverlapMaxFrames = overlapMaxFrames;
        Awake();
    }
    public void ChangeMusic(AudioClip clip, bool intro, bool playImmediately, bool overlap, int overlapMaxFrames)
    {
        StopAllCoroutines();
        pauseSecs = 0f;
        src.time = 0f;
        if (fadingOut && playImmediately)
        {
            fadingOut = false;
            Stop();
            src.volume = baseVolume;
        }
        if (IsPlaying())
        {
            Stop();
        }
        isPaused = false;
        musicName = name;

        hasIntro = intro;
        isPlaying = playImmediately;
        isOverlap = overlap;
        isOverlapFrames = 0;
        isOverlapMaxFrames = overlapMaxFrames;
        src.clip = clip;
        src.loop = !intro;
        src.Play();
    }
    public void ChangeMusic(string name, bool intro, bool playImmediately)
    {
        StopAllCoroutines();
        ChangeMusic(name, intro, playImmediately, overlap: false, 0);
    }

    public string CurrentMusic()
    {
        return musicName;
    }

    public void SetVolume(float volume)
    {
        baseVolume = volume;
    }

    public float GetVolume()
    {
        return baseVolume;
    }

    public void Pause()
    {
        if (!isPaused && isPlaying)
        {
            isPaused = true;
            pauseSecs = src.time;
            isPlaying = false;
            src.Stop();
        }
    }

    public void Resume()
    {
        if (isPaused && !isPlaying)
        {
            isPaused = false;
            src.Play();
            src.time = pauseSecs;
            isPlaying = true;
        }
    }

    public void FadeIn(float sec)
    {
        src.volume = 0f;
        frames = 1f / sec / 30f;
        fadingIn = true;
    }

    public AudioSource GetSource()
    {
        return src;
    }

    private IEnumerator PlayMusic()
    {
        if (hasIntro)
        {
            src.Play();
            yield return new WaitForSeconds(audioIntro.length);
            src.clip = audioLoop;
            src.loop = true;
        }
        if (!isPaused)
        {
            src.Play();
        }
        else
        {
            pauseSecs = 0f;
        }
        StopCoroutine(PlayMusic());
    }

    private IEnumerator NameScreen()
    {
        src.Play();
        yield return new WaitForSeconds(audioIntro.length);
        src2.Play();
        yield return new WaitForSeconds(audioIntro.length);
    }
}
