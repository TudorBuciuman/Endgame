using UnityEngine;

public class AnimatorEffectFunctions : MonoBehaviour
{
    [SerializeField]
    private bool destroyOnAudioSourceComplete;

    [SerializeField]
    private bool waitForAnimToConfirmFinished;

    private bool isFinished;

    private void Awake()
    {
        isFinished = !waitForAnimToConfirmFinished;
    }

    private void Update()
    {
        if ((bool)GetComponent<AudioSource>() && destroyOnAudioSourceComplete && !GetComponents<AudioSource>()[0].isPlaying && isFinished)
        {
            Object.Destroy(base.gameObject);
        }
    }

    public void MarkAsFinished()
    {
        isFinished = true;
    }

    public void PlayAudioSource(int num)
    {
        GetComponents<AudioSource>()[num].Play();
    }

    public void DestroyObject()
    {
        Object.Destroy(base.gameObject);
    }

    public void PlayNewAnimation(string animName)
    {
        GetComponent<Animator>().Play(animName, 0, 0f);
    }
}
