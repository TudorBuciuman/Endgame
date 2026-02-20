using UnityEngine;

public class SOULPiece : MonoBehaviour
{
    protected bool isMoving;

    protected float yspeed;

    private float xspeed;

    protected virtual void Awake()
    {
        isMoving = false;
    }

    protected virtual void Update()
    {
        if (isMoving)
        {
            base.transform.position += new Vector3(xspeed, yspeed);
            yspeed -= 0.006f;
        }
        if (base.transform.position.y < -10f)
        {
            Object.Destroy(base.gameObject);
        }
    }

    public void ChangeSOULColor(Color color)
    {
        base.gameObject.GetComponent<SpriteRenderer>().color = color;
    }

    public void StartMoving()
    {
        isMoving = true;
        yspeed = Random.Range(-0.0025f, 0.2f);
        xspeed = Random.Range(-0.2f, 0.2f);
    }
}
