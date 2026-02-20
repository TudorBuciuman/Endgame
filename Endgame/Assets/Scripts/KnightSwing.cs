using System;
using UnityEngine;

public class KnightSwing : AttackBase
{
    protected override void Awake()
    {
        base.Awake();
        maxFrames = 320;
        bbSize = new Vector2(165f, 140f);
    }
    public override void StartAttack()
    {
        base.StartAttack();
        UnityEngine.Object.FindObjectOfType<BladeKnight>().GetPart("arm").transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void OnDestroy()
    {
        if ((bool)UnityEngine.Object.FindObjectOfType<BladeKnight>())
        {
            UnityEngine.Object.FindObjectOfType<BladeKnight>().GetPart("arm").transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
            UnityEngine.Object.FindObjectOfType<BladeKnight>().GetPart("arm").transform.localEulerAngles = Vector3.zero;
        }
    }

    protected override void Update()
    {
        base.Update();
        if (!isStarted)
        {
            return;
        }
        int num = frames % 80;
        if (num < 45)
        {
            float num2 = (float)num / 30f;
            if (num2 < 1f)
            {
                num2 = Mathf.Sin(num2 * (float)Math.PI * 0.5f);
            }
            UnityEngine.Object.FindObjectOfType<BladeKnight>().GetPart("arm").transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Lerp(0f, 54f, num2) + Mathf.Sin((float)(num * 120) * ((float)Math.PI / 180f)) * (float)num / 45f);
            if (num > 10)
            {
                num2 = (float)(num - 10) / 20f;
                if (num2 < 0f)
                {
                    num2 = 0f;
                }
                float num3 = Mathf.Sin((float)(num * 36) * ((float)Math.PI / 180f)) / 2f + 1f;
                UnityEngine.Object.FindObjectOfType<BladeKnight>().GetPart("arm").transform.GetChild(0).localScale = new Vector3(num2, num2, 1f) * num3;
            }
        }
        else if (num < 50)
        {
            if (num == 46)
            {
                UnityEngine.Object.FindObjectOfType<BladeKnight>().GetPart("arm").transform.GetChild(0).localScale = new Vector3(0f, 0f, 0f);
                UnityEngine.Object.Instantiate(Resources.Load<GameObject>("battle/attacks/bullets/lostcore/BladeProjectileBullet")).GetComponent<BladeProjectileBullet>();
            }
            UnityEngine.Object.FindObjectOfType<BladeKnight>().GetPart("arm").transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Lerp(54f, -109.64f, (float)(num - 45) / 5f));
            UnityEngine.Object.FindObjectOfType<BladeKnight>().GetPart("swing").GetComponent<SpriteRenderer>()
                .color = Color.Lerp(new Color(1f, 0f, 0f, 0f), Color.red, (float)(num - 47) / 3f);
        }
        else if (num < 65)
        {
            float num4 = (float)(num - 50) / 15f;
            if (num4 < 1f)
            {
                num4 = Mathf.Sin(num4 * (float)Math.PI * 0.5f);
            }
            UnityEngine.Object.FindObjectOfType<BladeKnight>().GetPart("arm").transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Lerp(-109.64f, -115f, num4));
            UnityEngine.Object.FindObjectOfType<BladeKnight>().GetPart("swing").GetComponent<SpriteRenderer>()
                .color = Color.Lerp(Color.red, new Color(1f, 0f, 0f, 0f), num4);
        }
        else if (num < 80)
        {
            float num5 = (float)(num - 65) / 15f;
            num5 = num5 * num5 * num5 * (num5 * (6f * num5 - 15f) + 10f);
            UnityEngine.Object.FindObjectOfType<BladeKnight>().GetPart("arm").transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Lerp(-115f, 0f, num5));
        }
    }
}
