using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MATNet;

public class ChatSample : MonoBehaviour {

    public bool enable { get; set; }

    private Rect chatWindowRect = new Rect(Screen.width / 2 - 200, Screen.height / 2 - 200, 400, 400);
    private MNManager manager;

    public string messages = "";
    private string inputMesTemp = "";

	// Use this for initialization
	void Start () {
        enable = false;
        manager = MNManager.Instance;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnGUI()
    {
        if (!enable)
        {
            if (GUI.Button(new Rect(Screen.width/2-50, Screen.height/2-15, 100, 30), "Start"))
            {
                enable = true;
                manager.RegisterRpcMethod("EnterNewPlayer", this, "EnterNewPlayer");
                manager.RegisterRpcMethod("Chat", this, "Chat");
                manager.SendRPC("EnterNewPlayer", manager.player.displayName);
            }
        }
        else
        {
            chatWindowRect = GUI.Window(1, chatWindowRect, ChatWindow, "chat sample");
        }
    }

    void ChatWindow(int windowId)
    {
        GUI.TextArea(new Rect(20, 20, 360, 320), messages);
        inputMesTemp = GUI.TextArea(new Rect(20, 350, 250, 40), inputMesTemp);
        if (GUI.Button(new Rect(275, 350, 110, 40), "Send"))
        {
            manager.SendRPC("Chat", manager.player.displayName, inputMesTemp);
            inputMesTemp = "";
        }
    }

    public void EnterNewPlayer(string playerName)
    {
        messages += "Enter New Player: " + playerName + "\n";
    }

    public void Chat(string playerName, string message)
    {
        messages += playerName + ": " + message + "\n";
    }
}
