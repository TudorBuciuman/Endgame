using UnityEngine;

public abstract class SelectableBehaviour : MonoBehaviour
{
    public virtual void MakeDecision(Vector2 index, int id)
    {
        Debug.LogWarning("called base MakeDecision for some reason");
    }
}
