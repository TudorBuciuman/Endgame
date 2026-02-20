using UnityEngine;

public class SpecialAttackEffect : MonoBehaviour
{
    protected bool isPlaying = true;

    public virtual bool IsPlaying()
    {
        return isPlaying;
    }
}
