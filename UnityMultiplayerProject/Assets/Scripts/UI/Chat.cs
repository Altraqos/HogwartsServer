using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Chat : MonoBehaviour
{
    public int maxMessages = 30;
    public List<Message> messageList = new List<Message>();
    public GameObject chatPanel;
    public GameObject textObject;
    public InputField chatBox;

    public void Update()
    {
        if (chatBox.text != "")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SendMessage(chatBox.text);
                chatBox.text = "";
                chatBox.DeactivateInputField();
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            chatBox.ActivateInputField();
            chatBox.Select();
        }
    }

    public new void SendMessage(string msg)
    {
        string newstring = "#<color=green>#" + NetworkManager.instance.playerName + "#</color> #: " + msg;
        NetworkManager.instance.ForwardChat(newstring);
    }

    public void DisplayMessage(string msg)
    {
        if (messageList.Count >= maxMessages)
        {
            Destroy(messageList[0].textObject.gameObject);
            messageList.Remove(messageList[0]);
        }
        Message newMessage = new Message();
        newMessage.text = msg;
        GameObject newText = Instantiate(textObject, chatPanel.transform);
        newMessage.textObject = newText.GetComponent<Text>();
        newMessage.textObject.text = newMessage.text;
        messageList.Add(newMessage);
    }

}
[System.Serializable]
public class Message
{
    public string text;
    public Text textObject;
}
