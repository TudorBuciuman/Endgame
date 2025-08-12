using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private void Awake()
    {
        SetClamps(GameObject.Find("CameraBound_0").transform.position, GameObject.Find("CameraBound_1").transform.position);
        canvas = GameObject.Find("Canvas");
        GameManager gameManager = FindFirstObjectByType<GameManager>();
    }

    void Update()
    {
        base.transform.position = GetClampedPos();
        canvas.transform.position = new Vector3(this.transform.position.x, transform.position.y,0);
    }
    public void SetClamps(Vector3 topRight, Vector3 bottomLeft)
    {
        maxPos = topRight;
        minPos = bottomLeft;
    }
    public float GetZoneMusicPitch()
    {
        return zoneMusicSpeed;
    }

    public Vector3 GetClampedPos()
    {
        Transform transform = GameObject.Find("Player").transform;
        return new Vector3(Mathf.Clamp(transform.position.x, minPos[0] + 6f + 2f / 3f, maxPos[0] - 6f - 2f / 3f), Mathf.Clamp(transform.position.y, minPos[1] + 5f, maxPos[1] - 5f), -10f) + shakeOffset;
    }
}
