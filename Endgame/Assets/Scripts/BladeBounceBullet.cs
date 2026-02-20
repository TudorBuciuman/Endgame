using System;
using UnityEngine;

public class BladeBounceBullet : BulletBase
{
    private Vector3 dir;

    private float speed;

    private void Update()
    {
        base.transform.parent.position += new Vector3(dir.x * base.transform.parent.localScale.x, dir.y * base.transform.parent.localScale.y) * speed;
        if (Mathf.Abs(base.transform.parent.position.x) >= 6.66f && Mathf.Sign(base.transform.parent.position.x) == Mathf.Sign(base.transform.parent.localScale.x * dir.x))
        {
            base.transform.parent.localScale = new Vector3(0f - base.transform.parent.localScale.x, base.transform.parent.localScale.y);
            base.transform.localScale *= 0.75f;
        }
        if (Mathf.Abs(base.transform.parent.position.y) >= 5f && Mathf.Sign(base.transform.parent.position.y) == Mathf.Sign(base.transform.parent.localScale.y * dir.y))
        {
            base.transform.parent.localScale = new Vector3(base.transform.parent.localScale.x, 0f - base.transform.parent.localScale.y);
            base.transform.localScale *= 0.75f;
        }
        if (base.transform.localScale.x < 0.4f)
        {
            UnityEngine.Object.Destroy(base.transform.parent.gameObject);
        }
    }

    public void SetValues(float speed, float angle)
    {
        base.transform.localEulerAngles = new Vector3(0f, 0f, angle);
        dir = new Vector3(0f - Mathf.Sin(angle * ((float)Math.PI / 180f)), Mathf.Cos(angle * ((float)Math.PI / 180f))) / 48f;
        this.speed = speed;
    }
}
