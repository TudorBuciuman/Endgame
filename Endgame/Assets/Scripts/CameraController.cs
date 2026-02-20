using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{
    private GameObject canvas;
    private Vector2 maxPos;

    private Vector2 minPos;

    [SerializeField]
    private bool followPlayer = true;

    [SerializeField]
    private string zoneMusic;

    [SerializeField]
    private float zoneMusicSpeed = 1f;

    private Vector3 shakeOffset = Vector3.zero;

    private bool hitShake;

    private int shakeFrames;

    private float height = -10;
    private float Canvasheight = 0;
    private void Awake()
    {
        if(followPlayer)
        SetClamps(GameObject.Find("CameraBound_0").transform.position, GameObject.Find("CameraBound_1").transform.position);
        canvas = GameObject.Find("Canvas");
        height = transform.position.z;
        Canvasheight = canvas.transform.position.z;
        //GameManager gameManager = FindFirstObjectByType<GameManager>();
    }

    private void LateUpdate()
    {
        if (followPlayer)
        {
            Vector3 target = GetClampedPos();
            transform.position = SnapToPixelGrid(target);
            canvas.transform.position = new Vector3(this.transform.position.x, transform.position.y,Canvasheight);
        }
        if (hitShake && shakeFrames < 6)
        {
            shakeFrames++;
            if (shakeFrames == 6)
            {
                hitShake = false;
            }
            int num = Random.Range(-1, 2);
            int num2 = Random.Range(-1, 2);
            shakeOffset = new Vector3(num, num2) / 12f;
        }
        else
        {
            shakeOffset = Vector3.zero;
        }
        if (!Object.FindObjectOfType<BattleManager>())
        {
            Canvas[] array = Object.FindObjectsOfType<Canvas>();
            for (int i = 0; i < array.Length; i++)
            {
                array[i].transform.position = new Vector3((float)Mathf.RoundToInt(base.transform.position.x * 48f) / 48f, (float)Mathf.RoundToInt(base.transform.position.y * 48f) / 48f, Canvasheight);
            }
        }
    }
    public void SetClamps(Vector3 topRight, Vector3 bottomLeft)
    {
        maxPos = topRight;
        minPos = bottomLeft;
    }
    private Vector3 SnapToPixelGrid(Vector3 pos)
    {
        return pos;
    }
    public float GetZoneMusicPitch()
    {
        return zoneMusicSpeed;
    }

    public Vector3 GetClampedPos()
    {
        Transform transform = GameObject.Find("Player").transform;
        return new Vector3(Mathf.Clamp(transform.position.x, minPos[0] + 6f + 2f / 3f, maxPos[0] - 6f - 2f / 3f), Mathf.Clamp(transform.position.y, minPos[1] + 5f, maxPos[1] - 5f), height) + shakeOffset;
    }
    public string GetZoneMusic()
    {
        return "music/" + zoneMusic;
    }
    public void SetFollowPlayer(bool follow)
    {
        followPlayer = follow;
    }
    public void StartHitShake()
    {
        hitShake = true;
        shakeFrames = 0;
    }
}
