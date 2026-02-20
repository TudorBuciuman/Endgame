using UnityEngine;

public class PlayerAttackAnimation : MonoBehaviour
{
    protected bool isPlaying = true;

    protected virtual void Update()
    {
        if (!GetComponent<AudioSource>().isPlaying)
        {
            Object.Destroy(base.gameObject);
        }
    }

    public virtual void AssignValues(EnemyBase enemy, int partyMember, float targetExcellence, int partySize)
    {
        float num = 0.35f;
        float num2 = (0f - num) * ((float)partySize / 2f);
        base.transform.position = enemy.GetEnemyObject().transform.Find("atkpos").position + new Vector3(num2 + num * (float)partyMember + 0.15f, 0f);
        GetComponent<SpriteRenderer>().color = PartyPanels.defaultColors[partyMember];
        if (partyMember == 0 && (int)Util.GameManager().GetFlag(107) == 1 && (int)Util.GameManager().GetFlag(108) == 1)
        {
            GetComponent<SpriteRenderer>().color = new Color32(byte.MaxValue, 105, 105, byte.MaxValue);
        }
    }

    public void SetToStopPlaying()
    {
        isPlaying = false;
    }

    public bool IsPlaying()
    {
        return isPlaying;
    }
}
