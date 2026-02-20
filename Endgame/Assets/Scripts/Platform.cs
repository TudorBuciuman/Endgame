using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField]
    private int size = 1;

    private SpriteRenderer left;

    private SpriteRenderer mid;

    private SpriteRenderer right;

    private bool landed;

    [SerializeField]
    private float tpGain = 1f;

    [SerializeField]
    private float repeatTpGain = 0.25f;

    [SerializeField]
    private bool gainTPOnLand = true;

    private void Awake()
    {
        left = base.transform.Find("left").GetComponent<SpriteRenderer>();
        mid = base.transform.Find("mid").GetComponent<SpriteRenderer>();
        right = base.transform.Find("right").GetComponent<SpriteRenderer>();
        ChangeSize(size);
    }

    public void ChangeSize(int size)
    {
        this.size = size;
        left.transform.localPosition = new Vector3((float)(-size) / 96f, 0f);
        mid.transform.localScale = new Vector3(size, 1f, 1f);
        right.transform.localPosition = new Vector3((float)size / 96f, 0f);
        GetComponent<BoxCollider2D>().offset = new Vector2(0f, -0.007610887f);
        GetComponent<BoxCollider2D>().size = new Vector2((float)size / 48f, 0.07903963f);
    }

    public void Landed()
    {
        landed = true;
    }

    public float GetTPGain()
    {
        if (!gainTPOnLand)
        {
            return 0f;
        }
        if (landed)
        {
            return repeatTpGain;
        }
        return tpGain;
    }

    public void DisableGainTPOnLand()
    {
        gainTPOnLand = false;
    }

    public void OnDestroy()
    {
        if ((bool)GetComponentInChildren<SOUL>())
        {
            GetComponentInChildren<SOUL>().transform.parent = null;
        }
    }
}
