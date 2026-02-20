using UnityEngine;

public class BulletBoard : MonoBehaviour
{
    private int frames;

    private float rateX;

    private float rateY;

    private Transform up;

    private Transform left;

    private Transform right;

    private Transform down;

    private Transform bg;

    private Transform fakeup;

    private Transform fakeleft;

    private Transform fakeright;

    private Transform fakedown;

    private Transform fakeCornerUL;

    private Transform fakeCornerUR;

    private Transform fakeCornerBR;

    private Transform fakeCornerBL;

    private Vector2 curSize;

    private Vector2 oldSizeX;

    private Vector2 newSizeX;

    private Vector2 oldSizeY;

    private Vector2 newSizeY;

    private Vector2 oldPos;

    private Vector2 newPos;

    private Vector2 oldTPos;

    private Vector2 newTPos;

    private bool isPlaying;

    private void Awake()
    {
        up = base.transform.Find("Wall_UP");
        left = base.transform.Find("Wall_LEFT");
        right = base.transform.Find("Wall_RIGHT");
        down = base.transform.Find("Wall_DOWN");
        bg = base.transform.Find("Background");
        //if (!Object.FindObjectOfType<UnoBattleManager>())
        {
            fakeup = base.transform.Find("FakeWalls").Find("Wall_UP");
            fakeleft = base.transform.Find("FakeWalls").Find("Wall_LEFT");
            fakeright = base.transform.Find("FakeWalls").Find("Wall_RIGHT");
            fakedown = base.transform.Find("FakeWalls").Find("Wall_DOWN");
            //fakeCornerUL = base.transform.Find("FakeWalls").Find("Corner_UL");
            //fakeCornerUR = base.transform.Find("FakeWalls").Find("Corner_UR");
            //fakeCornerBR = base.transform.Find("FakeWalls").Find("Corner_BR");
            //fakeCornerBL = base.transform.Find("FakeWalls").Find("Corner_BL");
        }
        SpriteRenderer[] componentsInChildren;
        /*
        if ((int)Object.FindObjectOfType<GameManager>().GetFlag(94) == 1)
        {
            up.GetComponent<SpriteRenderer>().enabled = false;
            down.GetComponent<SpriteRenderer>().enabled = false;
            left.GetComponent<SpriteRenderer>().enabled = false;
            right.GetComponent<SpriteRenderer>().enabled = false;
            componentsInChildren = base.transform.Find("FakeWalls").GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                componentsInChildren[i].enabled = true;
            }
        }
        */
        componentsInChildren = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer spriteRenderer in componentsInChildren)
        {
            if (spriteRenderer.transform != bg)
            {
                spriteRenderer.color = Color.white;//UIBackground.borderColors[(int)Util.GameManager().GetFlag(223)];
            }
        }
        isPlaying = false;
        curSize = bg.localScale;
    }

    private void Update()
    {
        if (isPlaying)
        {
            frames++;
            bg.localScale = Vector2.Lerp(oldSizeX, newSizeX, (float)frames * rateX) + Vector2.Lerp(oldSizeY, newSizeY, (float)frames * rateY);
            bg.localPosition = Vector2.Lerp(oldPos, newPos, (float)frames * rateY);
            up.localScale = Vector2.Lerp(oldSizeX, newSizeX, (float)frames * rateX) + new Vector2(0f, 5f);
            up.localPosition = Vector2.Lerp(oldSizeY / 48f, newSizeY / 48f, (float)frames * rateY) + new Vector2(0f, -1.5104166f);
            left.localScale = Vector2.Lerp(oldSizeY, newSizeY, (float)frames * rateY) + new Vector2(5f, 0f);
            left.localPosition = Vector2.Lerp(oldSizeX / -2f / 48f, newSizeX / -2f / 48f, (float)frames * rateX) + Vector2.Lerp(oldSizeY / 2f / 48f, newSizeY / 2f / 48f, (float)frames * rateY) + new Vector2(5f / 96f, -1.4583334f);
            right.localScale = Vector2.Lerp(oldSizeY, newSizeY, (float)frames * rateY) + new Vector2(5f, 0f);
            right.localPosition = Vector2.Lerp(oldSizeX / 2f / 48f, newSizeX / 2f / 48f, (float)frames * rateX) + Vector2.Lerp(oldSizeY / 2f / 48f, newSizeY / 2f / 48f, (float)frames * rateY) + new Vector2(-5f / 96f, -1.4583334f);
            down.localScale = Vector2.Lerp(oldSizeX, newSizeX, (float)frames * rateX) + new Vector2(0f, 5f);
            base.transform.position = Vector2.Lerp(oldTPos, newTPos, (float)frames * rateY);
            //if (!Object.FindObjectOfType<UnoBattleManager>())
            {
                fakeup.localScale = up.localScale - new Vector3(10f, 0f);
                fakeup.localPosition = up.localPosition;
                fakedown.localScale = down.localScale - new Vector3(10f, 0f);
                fakedown.localPosition = down.localPosition;
                fakeleft.localScale = left.localScale - new Vector3(0f, 10f);
                fakeleft.localPosition = left.localPosition;
                fakeright.localScale = right.localScale - new Vector3(0f, 10f);
                fakeright.localPosition = right.localPosition;
                //fakeCornerUL.localPosition = new Vector2(fakeleft.localPosition.x - 1f / 48f - 0.0001f, fakeup.localPosition.y);
                //fakeCornerBL.localPosition = new Vector2(fakeleft.localPosition.x - 1f / 48f - 0.0001f, fakedown.localPosition.y - 1f / 48f);
                //fakeCornerUR.localPosition = new Vector2(fakeright.localPosition.x, fakeup.localPosition.y);
                //fakeCornerBR.localPosition = new Vector2(fakeright.localPosition.x, fakedown.localPosition.y - 1f / 48f);
            }
            if ((Vector2)bg.localScale == newSizeX + newSizeY)
            {
                curSize = newSizeX + newSizeY;
                isPlaying = false;
            }
            bg.localScale -= new Vector3(5f, 5f);
        }
    }

    public void StartMovement(Vector2 size, Vector2 TPos, bool instant = false)
    {
        if (!isPlaying)
        {
            frames = 0;
            if (instant)
            {
                frames = 420;
            }
            oldSizeX = new Vector2(curSize[0], 0f);
            newSizeX = new Vector2(size[0], 0f);
            rateX = 26f / Mathf.Abs(oldSizeX[0] - newSizeX[0]);
            oldSizeY = new Vector2(0f, curSize[1]);
            newSizeY = new Vector2(0f, size[1]);
            rateY = 24f / Mathf.Abs(oldSizeY[1] - newSizeY[1]);
            oldPos = new Vector2(0f, (curSize[1] - 140f) / 2f / 48f);
            newPos = new Vector2(0f, (size[1] - 140f) / 2f / 48f);
            oldTPos = base.transform.position;
            newTPos = TPos;
            isPlaying = true;
        }
    }

    public void SetBGOrder(int sortingOrder)
    {
        bg.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
    }

    public void StartMovement(Vector2 size, bool instant = false)
    {
        StartMovement(size, new Vector2(0f, -1.66f), instant);
    }

    public Vector2 GetCurrentSize()
    {
        return new Vector2(newSizeX.x, newSizeY.y);
    }

    public void ResetSize(bool instant = false)
    {
        StartMovement(new Vector2(575f, 140f), new Vector2(0f, -1.64f), instant);
    }

    public bool IsPlaying()
    {
        return isPlaying;
    }
}
