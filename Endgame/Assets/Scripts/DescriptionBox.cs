using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionBox : MonoBehaviour
{
    private List<Image> spriteElements;

    private Text description;

    private Text tpCost;

    private bool raised;

    private bool vanished = true;

    private void Awake()
    {
        spriteElements = new List<Image>();
        bool flag = (int)Util.GameManager().GetFlag(94) == 1;
        Color color = Color.white;
        Image[] componentsInChildren = GetComponentsInChildren<Image>();
        foreach (Image image in componentsInChildren)
        {
            if ((!flag && image.gameObject.name == "DescriptionBox") || (flag && image.transform.parent.gameObject.name.StartsWith("round")) || image.gameObject.name == "Inside")
            {
                spriteElements.Add(image);
                if (image.gameObject.name != "Inside")
                {
                    image.color = color;
                }
            }
            image.enabled = false;
        }
        description = base.transform.Find("Description").GetComponent<Text>();
        tpCost = base.transform.Find("TPCost").GetComponent<Text>();
        description.enabled = false;
        tpCost.enabled = false;
    }

    private void Update()
    {
        if (!vanished && Object.FindObjectOfType<BulletBoard>().IsPlaying())
        {
            Vanish();
        }
        float y = (raised ? (-172) : (-130));
        base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, new Vector3(0f, y), 0.5f);
    }

    public void Show()
    {
        raised = true;
    }

    public void Hide()
    {
        raised = false;
    }

    public void Vanish()
    {
        vanished = true;
        description.enabled = false;
        tpCost.enabled = false;
        foreach (Image spriteElement in spriteElements)
        {
            spriteElement.enabled = false;
        }
    }

    public void SetDescription(string description, string tpCost)
    {
        if (description == "" && tpCost == "")
        {
            Hide();
        }
        else
        {
            Show();
            this.description.text = description;
            this.tpCost.text = tpCost;
        }
        if (!vanished)
        {
            return;
        }
        vanished = false;
        this.description.enabled = true;
        this.tpCost.enabled = true;
        foreach (Image spriteElement in spriteElements)
        {
            spriteElement.enabled = true;
        }
    }
}
