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

    private Rect windowRect = new Rect(20, 20, 350, 300);

    private RoomDataWithId[] roomDataWithIds;
    private string[] roomSelStrings = new string[0];
    private int roomSelInt = 0;

    public string playerName = "";
    private string playerNameTemp = "Player";
    private bool debugMode;

    // Use this for initialization
    void Start()
    {
        enable = true;
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
            //Debug.Log(MNListServer.Instance.joiningRoomData.roomName);
            DrawJoiningRoomStatus();
        }
        else
        {
            DrawRoomSelect();
        }
    }

    void DrawEnterPlayerInfo()
    {
        GUI.Label(new Rect(20, 20, 300, 60), "Enter your name");
        playerNameTemp = GUI.TextField(new Rect(20, 80, 250, 40), playerNameTemp);
        debugMode = GUI.Toggle(new Rect(20, 130, 200, 50), debugMode, "debug mode");
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
        if (MNListServer.Instance.JoiningRoomData.Status == MNRoomData.RoomStatus.WaitingPlayer)
        {
            if (GUI.Button(new Rect(20, 70, 150, 30), "Start Session"))
            {
                MNListServer.Instance.StartSession();
            }
        }
        if (GUI.Button(new Rect(20, 120, 150, 30), "reflesh"))
        {
            RefleshRoomList();
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
