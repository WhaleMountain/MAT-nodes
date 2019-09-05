using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MATNet;

public class SimpleLobbyGUI : MonoBehaviour {

    public bool enable { get; set; }

    private Rect windowRect = new Rect(20, 20, 350, 300);

    private string[] roomSelStrings = new string[0];
    private MNRoomData[] roomSelDatas = new MNRoomData[0];
    private int roomSelInt = 0;

    public string playerName = "";
    private string playerNameTemp = "Player";

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(this);
        enable = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnGUI()
    {
        if (enable) {
            windowRect = GUI.Window(0, windowRect, RoomSelectionWindow, "Simple Lobby");
        }
    }

    void RoomSelectionWindow(int windowId)
    {
        if (playerName == "")
        {
            DrawEnterPlayerInfo();
        }
        else if (MNManager.Instance.CurrentStatus == MNManager.Status.WaitPlayer || MNManager.Instance.CurrentStatus == MNManager.Status.Playing)
        {
            DrawJoiningRoomStatus();
        }
        else if (MNManager.Instance.CurrentStatus == MNManager.Status.RoomSelecting)
        {
            DrawRoomSelect();
        }
    }

    void DrawEnterPlayerInfo()
    {
        GUI.Label(new Rect(20, 20, 300, 60), "Enter your name");
        playerNameTemp = GUI.TextField(new Rect(20, 80, 250, 40), playerNameTemp);
        if(GUI.Button(new Rect(80, 130, 100, 30), "Enter"))
        {
            playerName = playerNameTemp;
            MNManager.Instance.player.ChangeDisplayName(playerName);
        }
    }

    void DrawRoomSelect()
    {
        if (GUI.Button(new Rect(10, 20, 100, 30), "Join"))
        {
            JoinRoom(roomSelDatas[roomSelInt].roomID);
        }
        if (GUI.Button(new Rect(120, 20, 100, 30), "Create"))
        {
            CreateRoom("Room1");
        }
        if (GUI.Button(new Rect(240, 20, 100, 30), "Reflesh"))
        {
            RefleshRoomList();
        }
        roomSelInt = GUI.SelectionGrid(new Rect(10, 80, 210, 200), roomSelInt, roomSelStrings, 1);
    }

    void DrawJoiningRoomStatus()
    {
        GUI.Label(new Rect(20, 20, 300, 200), "You are joining room. ");
        if (MNManager.Instance.CurrentStatus == MNManager.Status.WaitPlayer)
        {
            if (GUI.Button(new Rect(20, 70, 150, 30), "Start Session"))
            {
                MNManager.Instance.StartSession();
            }
        }
    }

    void RefleshRoomList()
    {
        List<MNRoomData> roomDatas = MNManager.Instance.GetRoomList();
        List<string> roomStrings = new List<string>();
        foreach (MNRoomData data in roomDatas)
        {
            roomStrings.Add(data.roomName);
        }
        roomSelStrings = roomStrings.ToArray();
        roomSelDatas = roomDatas.ToArray();

        MNTools.DebugLog($"is valid: {MNManager.Instance.lobbyManager.IsValidMethod()}");
    }

    void JoinRoom(int id)
    {
        MNManager.Instance.JoinRoom(id);
    }

    void CreateRoom(string roomName)
    {
        MNManager.Instance.CreateRoom(roomName);
    }
}
