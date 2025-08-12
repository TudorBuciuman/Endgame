using UnityEngine;

public class InteractionTrigger : MonoBehaviour
{
    private int passAttempt;

    private bool triggering;

    private void Update()
    {
        passAttempt = 0;
        if (triggering)
        {
            base.transform.localPosition = Vector3.zero;
            triggering = false;
        }
        
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            //base.transform.localPosition = Object.FindObjectOfType<OverworldPlayer>().GetDirection() * 0.25f;
            triggering = true;
        }
        
    }

    private void AttemptInteract(Interactable interactable)
    {
        if (triggering)
        {
            triggering = false;
            interactable.DoInteract();
            base.transform.localPosition = Vector3.zero;
        }
    }

    public bool IsTriggering()
    {
        return triggering;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (triggering && (bool)collision.gameObject.GetComponent<Interactable>() && passAttempt < 1 && collision.gameObject.GetComponent<Interactable>().enabled )
        {
            passAttempt++;
            AttemptInteract(collision.gameObject.GetComponent<Interactable>());
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (triggering && (bool)collision.gameObject.GetComponent<Interactable>() && passAttempt < 1 && collision.gameObject.GetComponent<Interactable>().enabled) 
        { 
            passAttempt++;
            AttemptInteract(collision.gameObject.GetComponent<Interactable>());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggering && (bool)collision.gameObject.GetComponent<Interactable>() && passAttempt < 1 && collision.gameObject.GetComponent<Interactable>().enabled) { 
            passAttempt++;
            AttemptInteract(collision.gameObject.GetComponent<Interactable>());
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (triggering && (bool)collision.gameObject.GetComponent<Interactable>() && passAttempt < 1 && collision.gameObject.GetComponent<Interactable>().enabled )
        {
            passAttempt++;
            AttemptInteract(collision.gameObject.GetComponent<Interactable>());
        }
    }

}
