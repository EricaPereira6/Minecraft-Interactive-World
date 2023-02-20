using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextCanvas : MonoBehaviour
{
    public Text[] infoTexts;

    private int textIndex = 5;
    //private Color oldMessagesColor = Color.black;
    private Color oldMessagesColor = new Color(0.5f, 0.5f, 0.5f);

    static bool outsideRequester;
    static string requestedMessage;
    static Color requestedColor;

    void Start()
    {
        AddTextToScreen("Select E: microphone is off", Color.black);

        ResetRequest();
    }

    public void ResetRequest()
    {
        outsideRequester = false;
        requestedMessage = "";
        requestedColor = Color.black;
    }

    public void AddTextToScreen(string text, Color color)
    {
        if (textIndex > 0)
            textIndex--;

        for (int i = textIndex; i < infoTexts.Length - 1; i++)
        {
            infoTexts[i].text = infoTexts[i + 1].text;
            infoTexts[i].color = oldMessagesColor;
        }

        infoTexts[infoTexts.Length - 1].text = text;
        infoTexts[infoTexts.Length - 1].color = color;
    }

    public static void AddMessage(string text, Color color)
    {
        outsideRequester = true;
        requestedMessage = text;
        requestedColor = color;
    }

    void Update()
    {

        if (outsideRequester)
        {
            AddTextToScreen(requestedMessage, requestedColor);
            ResetRequest();
        }
    }
}
