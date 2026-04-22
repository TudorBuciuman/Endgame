using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeltaSelection : UIComponent
{
    private SelectableBehaviour interaction;

    private Vector2 index = Vector2.zero;

    private int selectID;

    private bool isActivated;

    private GameObject textboxGameObject;

    private Vector2 centerOffset = Vector2.zero;

    private Dictionary<Vector2, string> text = new Dictionary<Vector2, string>
    {
        {
            Vector2.up,
            ""
        },
        {
            Vector2.down,
            ""
        },
        {
            Vector2.left,
            ""
        },
        {
            Vector2.right,
            ""
        }
    };

    private Dictionary<Vector2, Vector3> offset = new Dictionary<Vector2, Vector3>
    {
        {
            Vector2.up,
            Vector3.zero
        },
        {
            Vector2.down,
            Vector3.zero
        },
        {
            Vector2.left,
            Vector3.zero
        },
        {
            Vector2.right,
            Vector3.zero
        }
    };

    private Dictionary<Vector2, bool> validPositions = new Dictionary<Vector2, bool>
    {
        {
            Vector2.up,
            false
        },
        {
            Vector2.down,
            false
        },
        {
            Vector2.left,
            false
        },
        {
            Vector2.right,
            false
        }
    };

    private Transform soul;

    private Dictionary<Vector2, Text> textObjects = new Dictionary<Vector2, Text>();

    private void Awake()
    {
        soul = base.transform.Find("SOUL");
        soul.GetComponent<Image>().color = Color.red;
        textObjects.Add(Vector2.left, base.transform.Find("Left").GetComponent<Text>());
        textObjects.Add(Vector2.right, base.transform.Find("Right").GetComponent<Text>());
        textObjects.Add(Vector2.up, base.transform.Find("Up").GetComponent<Text>());
        textObjects.Add(Vector2.down, base.transform.Find("Down").GetComponent<Text>());
    }

    private void Update()
    {
        if (!isActivated)
        {
            return;
        }
        if ((UTInput.GetAxis("Horizontal") == 0f || UTInput.GetAxis("Vertical") == 0f) && (UTInput.GetAxis("Horizontal") != 0f || UTInput.GetAxis("Vertical") != 0f))
        {
            Vector2 vector = index;
            Vector2 vector2 = Vector2.zero;
            if (UTInput.GetAxis("Horizontal") != 0f)
            {
                vector2 = new Vector2((int)UTInput.GetAxis("Horizontal"), 0f);
            }
            if (UTInput.GetAxis("Vertical") != 0f)
            {
                vector2 = new Vector2(0f, (int)UTInput.GetAxis("Vertical"));
            }
            if (vector2 != Vector2.zero && validPositions[vector2])
            {
                index = vector2;
            }
            if (vector != index)
            {
                if (vector != Vector2.zero)
                {
                    textObjects[vector].color = Color.white;
                }
                textObjects[index].color = Color.white;
                float x = textObjects[index].transform.localPosition.x - 18f;
                float y = textObjects[index].transform.localPosition.y + 55f;
                if (index == Vector2.up || index == Vector2.down)
                {
                    y = 44f * index.y;
                }
                soul.transform.localPosition = new Vector3(x, y);
            }
        }
        if ((Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || UTInput.GetButtonDown("Z")) && index != Vector2.zero && validPositions[index])
        {
            interaction.MakeDecision(index, selectID);
            if ((bool)textboxGameObject)
            {
                Object.Destroy(textboxGameObject);
            }
        }
    }

    public void Activate(SelectableBehaviour interaction, int selectID, GameObject textboxGameObject)
    {
        if (isActivated)
        {
            return;
        }
        this.interaction = interaction;
        this.selectID = selectID;
        base.transform.localPosition = base.transform.parent.GetChild(0).localPosition;
        foreach (Vector2 key in validPositions.Keys)
        {
            if (validPositions[key])
            {
                textObjects[key].text = Util.Unescape(text[key]);
                textObjects[key].transform.localPosition += offset[key];
                soul.localPosition = centerOffset;
            }
            else
            {
                textObjects[key].text = "";
            }
        }
        this.textboxGameObject = textboxGameObject;
        isActivated = true;
    }

    public void SetupChoice(Vector2 choicePos, string text, Vector3 offset)
    {
        if (validPositions.ContainsKey(choicePos))
        {
            this.text[choicePos] = text;
            this.offset[choicePos] = offset;
            validPositions[choicePos] = text != "";
        }
    }

    public void SetCenterOffset(Vector2 centerOffset)
    {
        this.centerOffset = centerOffset;
    }
}
