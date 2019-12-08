using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MATNodes;

public class SimpleGUI : MonoBehaviour
{
    class RoomDataWithId
    {
        public int id { get; set; }
        public MNRoomData data { get; set; }
    }

    public bool enable { get; set; }
    private bool roomCreateWindow;

    private Rect windowRect = new Rect(20, 20, 350, 300);
    private Rect rcwRect;

    private RoomDataWithId[] roomDataWithIds;
    private string[] roomSelStrings = new string[0];
    private int roomSelInt = 0;

    public string playerName = "";
    private string playerNameTemp = "Player";
    private string roomNameTemp = "Room";
    private bool debugMode;

    // Use this for initialization
    void Start()
    {
        enable = true;
        rcwRect = new Rect(Screen.width / 2 - 100, Screen.height / 2 - 50, 200, 100);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnGUI()
    {
        if (enable)
        {
            windowRect = GUI.Window(0, windowRect, RoomSelectionWindow, "Simple Lobby");
            if (roomCreateWindow)
            {
                rcwRect = GUI.ModalWindow(1, rcwRect, CreateRoomWindow, "Create Room");
            }
        }
    }

    void RoomSelectionWindow(int windowId)
    {
        if (playerName == "")
        {
            DrawEnterPlayerInfo();
        }
        else if (MNListServer.Instance.OnRoom)
        {
            DrawJoiningRoomStatus();
        }
        else
        {
            DrawRoomSelect();
        }
    }

    void CreateRoomWindow(int windowId)
    {
        GUILayout.Label("Enter room name.");
        roomNameTemp = GUILayout.TextField(roomNameTemp);
        if (GUILayout.Button("Create"))
        {
            roomCreateWindow = false;
            CreateRoom(roomNameTemp);
            roomNameTemp = "Room";
        }
    }

    void DrawEnterPlayerInfo()
    {
        GUI.Label(new Rect(20, 20, 300, 60), "Enter your name");
        playerNameTemp = GUI.TextField(new Rect(20, 80, 250, 40), playerNameTemp);
        debugMode = GUI.Toggle(new Rect(20, 250, 200, 50), debugMode, "debug mode");
        if (GUI.Button(new Rect(140, 130, 100, 30), "Enter"))
        {
            playerName = playerNameTemp;
            MNListServer.Instance.Player = new MNPlayer(playerName);
            if (debugMode) MNListServer.Instance.Player.canHost = true;//for debug
        }
    }

    void DrawRoomSelect()
    {
        if (GUI.Button(new Rect(10, 20, 100, 30), "Join"))
        {
            JoinRoom(roomDataWithIds[roomSelInt].id);
        }
        if (GUI.Button(new Rect(120, 20, 100, 30), "Create"))
        {
            roomCreateWindow = true;
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
        if (MNListServer.Instance.JoiningRoomData.Status == MNRoomData.RoomStatus.WaitingPlayer)
        {
            string members = "Members:\n";
            foreach (MNPlayer player in MNListServer.Instance.JoiningRoomData.Players)
            {
                members += player.displayName + "\n";
            }
            GUI.TextArea(new Rect(20, 40, 300, 200), members);
            if (GUI.Button(new Rect(20, 250, 150, 30), "Start Session"))
            {
                MNListServer.Instance.StartSession();
            }
        }
    }

    void RefleshRoomList()
    {
        Dictionary<int, MNRoomData> dataDict = MNListServer.Instance.GetRoomList();
        List<RoomDataWithId> temp = new List<RoomDataWithId>();
        List<string> stringsTemp = new List<string>();
        foreach (KeyValuePair<int, MNRoomData> kvp in dataDict)
        {
            temp.Add(new RoomDataWithId { id = kvp.Key, data = kvp.Value });
            stringsTemp.Add(kvp.Value.RoomName);
        }
        roomDataWithIds = temp.ToArray();
        roomSelStrings = stringsTemp.ToArray();
    }

    void JoinRoom(int id)
    {
        MNListServer.Instance.JoinRoom(id);
    }

    void CreateRoom(string roomName)
    {
        MNListServer.Instance.CreateRoom(roomName);
    }
}
