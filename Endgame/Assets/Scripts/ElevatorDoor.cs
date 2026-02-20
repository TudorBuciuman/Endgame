using System;
using UnityEngine;

public class ElevatorDoor : Interactable
{
    private bool activated;

    private int frames;

    private void Awake()
    {
        for (int i = 0; i < 3; i++)
        {
            base.transform.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder - ((i != 0) ? 1 : 50);
        }
    }
    private void Update()
    {
        if (activated)
        {
            frames++;
            for (int i = 1; i <= 2; i++)
            {
                float num = ((i != 1) ? 1 : (-1));
                base.transform.GetChild(i).localPosition = new Vector3(Mathf.Lerp(0f, 0.81f, (float)frames / 20f) * num, 0f);
            }
            if (frames == 20)
            {
                activated = false;
                GetComponent<BoxCollider2D>().enabled = false;
            }
        }
    }

    public override void DoInteract()
    {
        if (!activated)
        {
            activated = true;
            GetComponent<AudioSource>().Play();
        }
    }

    public override int GetEventData()
    {
        throw new NotImplementedException();
    }

    public override void MakeDecision(Vector2 index, int id)
    {
        throw new NotImplementedException();
    }
}
