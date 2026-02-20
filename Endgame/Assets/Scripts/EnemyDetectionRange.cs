using UnityEngine;

public class EnemyDetectionRange : MonoBehaviour
{
    private OverworldEnemyBase enemy;

    public void SetParentEnemy(OverworldEnemyBase enemy)
    {
        this.enemy = enemy;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((bool)collision.gameObject.GetComponent<PlayerController>())
        {
            enemy.DetectPlayer();
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    public void ActivateTrigger()
    {
        GetComponent<BoxCollider2D>().enabled = true;
    }

    public void DeactivateTrigger()
    {
        GetComponent<BoxCollider2D>().enabled = false;
    }
}
