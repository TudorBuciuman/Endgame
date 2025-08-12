using UnityEngine;

public class LostCoreMusic : MonoBehaviour
{
    private AudioSource main;

    private AudioSource battle;

    private bool danger;

    private float dangerMulti;

    private float volume = 1f;

    private bool fadingOut;

    private int fadeOutFrames;

    private int maxFOFrames = 30;

    private void Awake()
    {
        main = GetComponents<AudioSource>()[0];
        battle = GetComponents<AudioSource>()[1];
    }

    private void Update()
    {
        if (main.time != battle.time)
        {
            battle.time = main.time;
        }
        if (danger && dangerMulti < 1f)
        {
            dangerMulti += 1f / 6f;
        }
        else if (!danger && dangerMulti > 0f)
        {
            dangerMulti -= 1f / 60f;
        }
        main.volume = volume;
        battle.volume = volume * dangerMulti;
        if (fadingOut)
        {
            fadeOutFrames++;
            float num = Mathf.Lerp(1f, 0f, (float)fadeOutFrames / (float)maxFOFrames);
            main.volume *= num;
            battle.volume *= num;
            if (fadeOutFrames >= maxFOFrames)
            {
                GetComponentInParent<MusicPlayer>().Stop();
            }
        }
    }

    public void FadeOut(int frames)
    {
        fadingOut = true;
        fadeOutFrames = 0;
        maxFOFrames = frames;
    }

    public void SetDanger(bool danger)
    {
        this.danger = danger;
    }
}
