using UnityEngine;

public class SpecialACT : MonoBehaviour
{
    protected bool activated;

    public virtual bool IsActivated()
    {
        return activated;
    }

    public virtual void Activate()
    {
        activated = true;
    }
}
