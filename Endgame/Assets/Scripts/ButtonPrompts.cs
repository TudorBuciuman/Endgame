using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPrompts : MonoBehaviour
{
    private List<Image> buttons;

    public static string[] validButtons = new string[]
    {
        "South", "East", "West", "North",
        "LeftShoulder", "RightShoulder",
        "Select", "Start",
        "LeftStick", "RightStick",
        "DpadUp", "DpadDown", "DpadLeft", "DpadRight"
    };

    public static Dictionary<int, string[]> buttonDict = new Dictionary<int, string[]>
    {
        { 0, new string[] { "up", "?" } },
        { 1, new string[] { "down", "?" } },
        { 2, new string[] { "left", "?" } },
        { 3, new string[] { "right", "?" } },
        { 4, new string[] { "y", "?" } },
        { 5, new string[] { "b", "?" } },
        { 6, new string[] { "a", "\uff00" } },
        { 7, new string[] { "x", "?" } },
        { 8, new string[] { "left_stick", "?" } },
        { 9, new string[] { "right_stick", "?" } },
        { 10, new string[] { "left_bumper", "?" } },
        { 11, new string[] { "right_bumper", "?" } },
        { 12, new string[] { "start", "?" } },
        { 13, new string[] { "select", "?" } },
        { -1, new string[] { "questionmark", "\uffff" } }
    };


    private void Awake()
    {
        buttons = new List<Image>();
    }
    /*
    public void AddPrompt(RectTransform parent, float x, float y, string button, int size)
    {
        string spriteName = GetButtonGraphic(button);
        Image image = new GameObject("button_" + button).AddComponent<Image>();
        image.sprite = Resources.Load<Sprite>("ui/buttons/" + spriteName);
        image.rectTransform.SetParent(parent);
        image.rectTransform.localScale = Vector3.one;

        float offset = (size != 2) ? 9f : 0f;
        image.rectTransform.localPosition = new Vector2(
            Mathf.Round(parent.rect.width / -2f) + 16f + x,
            Mathf.Round(parent.rect.height / 2f) - 16f + y + offset
        );
        image.rectTransform.sizeDelta = image.sprite.textureRect.size * size;

        buttons.Add(image);
    }
    */
    public void DeleteButtons()
    {
        foreach (var button in buttons)
        {
            if (button != null)
                Destroy(button.gameObject);
        }
        buttons.Clear();
    }
}
