using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Chat : MonoBehaviour {
    public InputField input;
    public GameObject chatInput;
    private int height = 40;
    private int i = 0;

	public void sendChat()
    {
        if (!string.IsNullOrEmpty(input.text.Trim()))
        {
            GameObject text = new GameObject("Message");
            text.AddComponent<Text>();
            text.GetComponent<Text>().font = input.GetComponentInChildren<Text>().font;
            text.GetComponent<Text>().fontSize = 35;
            text.GetComponent<RectTransform>().sizeDelta = new Vector2(500, height);
            text.GetComponent<Text>().text += Camera.main.gameObject.GetComponentInParent<Character>().getCName() + ": " + input.text;
            text.transform.SetParent(chatInput.transform, false);
            chatInput.GetComponent<RectTransform>().sizeDelta = new Vector2(0, chatInput.transform.childCount * 40);
        }
        input.text = "";
        input.DeactivateInputField();
        Camera.main.GetComponent<PlayerCamera>().endChatting();
    }

    public void sendSystemMessage(string message)
    {
        GameObject text = new GameObject("System");
        text.AddComponent<Text>();
        text.GetComponent<Text>().font = input.GetComponentInChildren<Text>().font;
        text.GetComponent<Text>().fontSize = 35;
        text.GetComponent<Text>().color = Color.red;
        text.GetComponent<RectTransform>().sizeDelta = new Vector2(500, height);
        text.GetComponent<Text>().text += "System: " + message;
        text.transform.SetParent(chatInput.transform, false);
        chatInput.GetComponent<RectTransform>().sizeDelta = new Vector2(0, chatInput.transform.childCount * 40);
    }
}
